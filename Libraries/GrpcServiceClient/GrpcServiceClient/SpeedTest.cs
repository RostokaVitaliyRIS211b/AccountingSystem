using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient
{
    using System.IO;
    using System.Net;
    using System.Net.Security;

    public class ThrottlingStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly long _bytesPerSecond;
        private readonly int _latencyMs;

        public ThrottlingStream(Stream innerStream, long bytesPerSecond, int latencyMs = 0)
        {
            _innerStream = innerStream;
            _bytesPerSecond = bytesPerSecond;
            _latencyMs = latencyMs;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Симуляция задержки сети (Latency) перед началом чтения
            if (_latencyMs > 0)
            {
                await Task.Delay(_latencyMs, cancellationToken);
            }

            int bytesRead = await _innerStream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

            // Симуляция ограничения скорости (Bandwidth)
            if (_bytesPerSecond > 0 && bytesRead > 0)
            {
                // Сколько времени должно занять чтение этих байт
                long expectedMs = (bytesRead * 1000) / _bytesPerSecond;
                if (expectedMs > 0)
                {
                    await Task.Delay((int)expectedMs, cancellationToken);
                }
            }

            return bytesRead;
        }

        // Остальные методы просто делегируем внутреннему потоку
        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;
        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }
        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            if (disposing) _innerStream.Dispose();
            base.Dispose(disposing);
        }
    }



    public class ThrottlingHttpMessageHandler : DelegatingHandler
    {
        private readonly long _bytesPerSecond;
        private readonly int _latencyMs;

        public ThrottlingHttpMessageHandler(long bytesPerSecond, int latencyMs = 0, SocketsHttpHandler? handler = null)
        {
            _bytesPerSecond = bytesPerSecond;
            _latencyMs = latencyMs;
            InnerHandler = handler ?? new SocketsHttpHandler()
            {
                EnableMultipleHttp2Connections = true,

                AutomaticDecompression = DecompressionMethods.GZip,
            }; 
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Получаем ответ от нижнего обработчика (например, SocketsHttpHandler)
            var response = await base.SendAsync(request, cancellationToken);

            if (response.Content != null)
            {
                var originalStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var throttledStream = new ThrottlingStream(originalStream, _bytesPerSecond, _latencyMs);

                var newContent = new StreamContent(throttledStream);

                // 🔥 Копируем ВСЕ заголовки из оригинального контента
                foreach (var header in response.Content.Headers)
                {
                    // TryAddWithoutValidation позволяет добавить заголовки, которые обычно защищены
                    newContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                // Также копируем заголовки ответа (не контента), если нужно
                foreach (var header in response.Headers)
                {
                    response.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                response.Content = newContent;
            }

            return response;
        }
    }
}
