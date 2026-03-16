using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ObjectsManager.Helpers
{
    public static class AppSettingsHelper
    {
        public static AppSettings Settings { get; private set; }

        private static string PathSettings { get; } = "";

        static AppSettingsHelper()
        {
            try
            {
                var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(currentDir, "appsettings.json");
                PathSettings = path;
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(PathSettings), options);
                Settings = settings!;
            }
            catch
            {

            }

            Settings ??= new("127.0.0.1", "5001", "", "");
        }

        public static void SaveSettings(AppSettings settings)
        {
            Settings = settings;
            try
            {
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(PathSettings, json);
            }
            catch
            {

            }
        }
    }

    public class AppSettings
    {
        public AppSettings(string IpAddress, string Port, string UserName, string password, bool IsCachingOn = false)
        {
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.UserName = UserName;
            this.IsCachingOn = IsCachingOn;
            Password = password;
        }

        [JsonPropertyName("IP Адрес")]
        public string IpAddress { get; }

        [JsonPropertyName("Порт")]
        public string Port { get; }

        [JsonPropertyName("Пользователь")]
        public string UserName { get; }

        [JsonPropertyName("Кэширование")]
        public bool IsCachingOn { get; }

        [JsonIgnore]
        [XmlIgnore]
        public string Password { get; }

        public AppSettings Copy(bool IsCachingOn) => new(IpAddress, Port, UserName, Password, IsCachingOn);
    }


}
