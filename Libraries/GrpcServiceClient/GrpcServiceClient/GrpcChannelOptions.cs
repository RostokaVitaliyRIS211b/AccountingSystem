using Grpc.Net.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcServiceClient
{
    internal static class GrpcChannelOptionsHelper
    {
        public const string Address = "http://localhost:5001";
        public const int MaxReceiveMessageSizeConst = 100 * 1024 * 1024;//100 megabytes
        public const int MaxSendMessageSizeConst = 100 * 1024 * 1024;//100 megabytes
        public static GrpcChannelOptions GetGrpcChannelOptions(string username, string password, string address = Address)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(address);
            var service = new AuthService.AuthServiceClient(channel);
            var reply3 = service.Authentificate(new AuthRequest() { Password = password, Username = username });
            HttpClient httpClientWithAuth = new HttpClient();
            httpClientWithAuth.DefaultRequestHeaders.Add("Authorization", $"Bearer {reply3.Token}");
            httpClientWithAuth.DefaultRequestHeaders.Add("Id",Guid.NewGuid().ToString());
            GrpcChannelOptions options = new()
            {
                HttpClient = httpClientWithAuth,
                MaxReceiveMessageSize = MaxReceiveMessageSizeConst,
                MaxSendMessageSize = MaxSendMessageSizeConst
            };
            return options;
        }
    }
}
