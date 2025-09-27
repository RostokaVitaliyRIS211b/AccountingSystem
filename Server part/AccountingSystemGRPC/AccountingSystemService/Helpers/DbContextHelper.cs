using BdClasses;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace AccountingSystemService.Helpers
{
    public static class DbContextHelper
    {
        public static string? Connection { get; set; }
        public static string? Password { get; set; }
        public static string? Username { get; set; }
        public static string? DatabaseName { get; set; }

        private static string bDir = "";
        public static string BackupDir
        {
            get
            {
                if (string.IsNullOrEmpty(bDir))
                {
                    bDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups");
                    if (!Directory.Exists(bDir))
                    {
                        Directory.CreateDirectory(bDir);
                    }
                }
                return bDir;
            }

        }

        private static string pgDump = "";
        public static string PgDumpPath
        {
            get
            {
                if (string.IsNullOrEmpty(pgDump))
                {
                    pgDump = "pg_dump";
                }
                return pgDump;
            }
        }


        static DbContextHelper()
        {
            string json = File.ReadAllText("appsettings.json");
            using JsonDocument doc = JsonDocument.Parse(json);
            Connection = doc.RootElement.GetProperty("ConnectionStrings").GetProperty("DatabaseConnection").GetString();
            if (Connection != null)
            {
                var paramsOfCon = ParseConnectionString(Connection);

                if(paramsOfCon.TryGetValue("Password", out var password))
                {
                    Password = password;
                }

                if(paramsOfCon.TryGetValue("Username",out var username))
                {
                    Username = username;
                }

                if(paramsOfCon.TryGetValue("Database", out var database))
                {
                    DatabaseName = database;
                }
            }
        }

        public static void ProcessOptionsBuilder(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql(Connection);
        }

        public static ConstructionContext GetConstructionContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ConstructionContext>();
            ProcessOptionsBuilder(optionsBuilder);
            var options = optionsBuilder.Options;
            return new(options);
        }

        private static Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            var parameters = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(connectionString))
                return parameters;

            // Разделяем строку по точке с запятой
            string[] pairs = connectionString.Split(';');

            foreach (string pair in pairs)
            {
                // Убираем пробелы и проверяем, что пара не пустая
                var mypair = pair.Trim();
                if (string.IsNullOrEmpty(pair))
                    continue;

                // Разделяем по первому знаку '='
                int equalsIndex = mypair.IndexOf('=');
                if (equalsIndex == -1)
                    continue; // Пропускаем невалидные пары

                string key = mypair[..equalsIndex].Trim();
                string value = mypair[(equalsIndex + 1)..].Trim();

                parameters[key] = value;
            }

            return parameters;
        }
    }
}
