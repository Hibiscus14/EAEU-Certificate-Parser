using EaeuCertificateParser.Database;
using Microsoft.Extensions.Configuration;

namespace EaeuCertificateParser
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

            var dbSettings = configuration.GetSection("Database").Get<DatabaseSettings>();

            string connectionString;

            if (dbSettings.Provider == "PostgreSQL")
            {
                connectionString = dbSettings.PostgreSQL.ConnectionString;
            }
            else
            {
                connectionString = dbSettings.Sqlite.ConnectionString;
            }

            var dbService = new DbService(dbSettings.Provider, connectionString);

            var apiClient = new ApiClient();
            await apiClient.ParseAndSaveAllPagesAsync(dbService);

            Console.WriteLine("Парсинг завершён!");
        }
    }
}
