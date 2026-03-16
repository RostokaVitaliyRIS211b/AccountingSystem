using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcServiceClient
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly string _token;
        private readonly string _id;
        public AuthHeaderHandler(string token, string id, HttpMessageHandler innerHandler)
        {
            _token = token;
            _id = id;
            InnerHandler = innerHandler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", $"Bearer {_token}");
            request.Headers.Add("Id", _id);
            return base.SendAsync(request, cancellationToken);
        }
    }

    internal static class GrpcChannelOptionsHelper
    {
        public const string Address = "http://localhost:5001";
        public const int MaxReceiveMessageSizeConst = 100 * 1024 * 1024;//100 megabytes
        public const int MaxSendMessageSizeConst = 100 * 1024 * 1024;//100 megabytes
        public static GrpcChannelOptions GetGrpcChannelOptions(string username, string password, string address = Address,long internetSpeed = long.MaxValue, int latencyMs = 0)
        {
            var Id = Guid.NewGuid().ToString();
            GrpcChannel channel = GrpcChannel.ForAddress(address);
            var service = new AuthService.AuthServiceClient(channel);
            var metadata = new Metadata()
            {
                { "id",Id }
            };
            var reply3 = service.Authentificate(new AuthRequest() { Password = password, Username = username },headers:metadata);
            channel.ShutdownAsync();
            //var socketsHandler = new SocketsHttpHandler
            //{
            //    EnableMultipleHttp2Connections = true,
            //    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            //};

            //// 3. Собираем цепочку хендлеров: Auth -> Throttling -> Sockets
            //HttpMessageHandler handler = socketsHandler;

            //if (latencyMs != 0 || internetSpeed != long.MaxValue)
            //{
            //    handler = new ThrottlingHttpMessageHandler(internetSpeed, latencyMs, socketsHandler);
            //}
            

            var httpClientWithAuth = new HttpClient();
            httpClientWithAuth.DefaultRequestHeaders.Add("Authorization", $"Bearer {reply3.Token}");
            httpClientWithAuth.DefaultRequestHeaders.Add("Id", Id);

            GrpcChannelOptions options = new()
            {
                HttpClient = httpClientWithAuth,
                //HttpHandler = new AuthHeaderHandler(reply3.Token, Id, handler),
                MaxReceiveMessageSize = MaxReceiveMessageSizeConst,
                MaxSendMessageSize = MaxSendMessageSizeConst,
                CompressionProviders = [new GzipCompressionProvider(System.IO.Compression.CompressionLevel.SmallestSize)]
            };
           
            return options;
        }
    }
}
