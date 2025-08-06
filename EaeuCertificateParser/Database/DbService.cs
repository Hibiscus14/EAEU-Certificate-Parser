using EaeuCertificateParser.Models;
using Microsoft.Data.Sqlite;
using Npgsql;
using SQLitePCL;
using System.Data;

public class DbService
{
    private readonly string _connectionString;
    private readonly string _provider;

    public DbService(string provider, string connectionString)
    {
        _provider = provider;
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        if (_connectionString == "Sqlite")
        {
            Batteries.Init();
            InitializeDatabase();
        }
        return _provider switch
        {
            "PostgreSQL" => new NpgsqlConnection(_connectionString),
            "Sqlite" => new SqliteConnection(_connectionString),
            _ => throw new InvalidOperationException("Unsupported database provider")
        };
    }
    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS certificates (
                doc_id TEXT PRIMARY KEY,
                country_code TEXT,
                status_code TEXT,
                applicant TEXT,
                manufacturer TEXT,
                doc_kind TEXT,
                start_date TEXT,
                validity_date TEXT
            );
        ";

        command.ExecuteNonQuery();
    }

    public void InsertCertificate(Certificate cert)
    {
        var conn = CreateConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO certificates (doc_id, country_code, status_code, applicant, manufacturer, doc_kind, start_date, validity_date)
            VALUES (@doc_id, @country_code, @status_code, @applicant, @manufacturer, @doc_kind, @start_date, @validity_date)
            ON CONFLICT(doc_id) DO NOTHING;";

        AddParameter(cmd, "@doc_id", cert.DocId);
        AddParameter(cmd, "@country_code", cert.CountryCode ?? "");
        AddParameter(cmd, "@status_code", cert.StatusCode ?? "");
        AddParameter(cmd, "@applicant", cert.Applicant ?? "");
        AddParameter(cmd, "@manufacturer", cert.Manufacturer ?? "");
        AddParameter(cmd, "@doc_kind", cert.DocKind ?? "");
        AddParameter(cmd, "@start_date", cert.StartDate);
        AddParameter(cmd, "@validity_date", cert.ValidityDate is DateTime dt ? dt : DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}
