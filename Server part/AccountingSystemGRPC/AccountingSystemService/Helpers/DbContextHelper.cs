using BdClasses;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace AccountingSystemService.Helpers
{
    public static class DbContextHelper
    {
        public static string? Connection { get; set; }
        static DbContextHelper()
        {
            string json = File.ReadAllText("appsettings.json");
            using JsonDocument doc = JsonDocument.Parse(json);
            Connection = doc.RootElement.GetProperty("ConnectionStrings").GetProperty("DatabaseConnection").GetString();
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


    }
}
