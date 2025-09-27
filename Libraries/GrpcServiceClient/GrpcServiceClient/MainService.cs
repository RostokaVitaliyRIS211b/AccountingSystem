using Grpc.Net.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcServiceClient
{
    public partial class MainService : IDisposable
    {
        public MainService(string name, string password, string address = GrpcChannelOptionsHelper.Address)
        {
            Channel = GrpcChannel.ForAddress(address, GrpcChannelOptionsHelper.GetGrpcChannelOptions(name, password, address));
            Client = new AccountingSystem.AccountingSystemClient(Channel);
            Address = address;
            Username = name;
            Password = password;
            try
            {
                Client.AddSessionData(new Google.Protobuf.WellKnownTypes.Empty());
            }
            catch
            {
               
            }
        }

        private GrpcChannel Channel { get; }

        private AccountingSystem.AccountingSystemClient Client { get; }

        public string Address { get; }

        public string Username { get; }

        public string Password { get; }

        public void Dispose()
        {
            try
            {
                Client.AbortSessionData(new Google.Protobuf.WellKnownTypes.Empty());
            }
            catch
            {

            }
            Channel.Dispose();
        }
    }
}
