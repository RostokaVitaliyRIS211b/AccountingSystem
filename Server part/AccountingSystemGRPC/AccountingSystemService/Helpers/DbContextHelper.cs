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
        public static ConstructionContext GetConstructionContext()
        {
            var  optionsBuilder = new DbContextOptionsBuilder<ConstructionContext>();
            var options = optionsBuilder.UseNpgsql(Connection).Options;
            return new(options);
        }
    }
}
