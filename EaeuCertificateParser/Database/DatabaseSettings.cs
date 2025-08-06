namespace EaeuCertificateParser.Database
{
    public class DatabaseSettings
    {
        public string Provider { get; set; } = "Sqlite";
        public ConnectionStrings Sqlite { get; set; }
        public ConnectionStrings PostgreSQL { get; set; }
    }

    public class ConnectionStrings
    {
        public string ConnectionString { get; set; }
    }
}
