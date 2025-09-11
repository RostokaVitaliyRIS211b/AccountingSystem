using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ObjectsManager.Helpers
{
    public static class ConnectionSettingsHelper
    {
        public static ConnectionSettings Settings { get; }

        static ConnectionSettingsHelper()
        {
            var currentFir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(currentFir, "connectionSettings.json");
            var settings = JsonSerializer.Deserialize<ConnectionSettings>(path);
            if(settings is not null)
            {
                Settings = settings;
            }
            Settings ??= new("127.0.0.1", "","5001","");
        }

        
    }

    public class ConnectionSettings
    {
        public ConnectionSettings(string IpAddress, string Password, string Port, string UserName)
        {
            this.IpAddress = IpAddress;
            this.Password = Password;
            this.Port = Port;
            this.UserName = UserName;
        }

        public string IpAddress { get; }

        public string Password { get; }
        public string Port { get; }

        public string UserName { get; }
    }


}
