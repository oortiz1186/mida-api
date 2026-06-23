using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SoporteMida.Api.Config;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;

namespace SoporteMida.Api.Integrations.Contpaqi.Services;

public class ContpaqiSqlService
{

    public async Task<List<ContpaqiCustomerDto>> GetCustomersAsync(string databaseName)
    {
        var customers = new List<ContpaqiCustomerDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            CIDCLIENTEPROVEEDOR,
            CCODIGOCLIENTE,
            CRAZONSOCIAL,
            CRFC,
            CEMAIL1,
            CUSOCFDI,
            CREGIMFISC,
            CWHATSAPP,
            CESTATUS
        FROM admClientes
        WHERE CIDCLIENTEPROVEEDOR > 0
        ORDER BY CRAZONSOCIAL";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            customers.Add(new ContpaqiCustomerDto
            {
                Id = Convert.ToInt32(reader["CIDCLIENTEPROVEEDOR"]),
                Codigo = reader["CCODIGOCLIENTE"]?.ToString()?.Trim() ?? "",
                RazonSocial = reader["CRAZONSOCIAL"]?.ToString()?.Trim() ?? "",
                Rfc = reader["CRFC"]?.ToString()?.Trim(),
                Email = reader["CEMAIL1"]?.ToString()?.Trim(),
                UsoCfdi = reader["CUSOCFDI"]?.ToString()?.Trim(),
                RegimenFiscal = reader["CREGIMFISC"]?.ToString()?.Trim(),
                Whatsapp = reader["CWHATSAPP"]?.ToString()?.Trim(),
                Estatus = Convert.ToInt32(reader["CESTATUS"])
            });
        }

        return customers;
    }

    public async Task<List<ContpaqiCompanyDto>> GetCompaniesAsync()
    {
        var companies = new List<ContpaqiCompanyDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "DB_Directory"
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            idDataBase,
            GuidCompany,
            Version,
            NombreEmpresa,
            Alias,
            RFC,
            CompanyPath,
            DB_DocumentsMetadata,
            DB_DocumentsContent
        FROM DatabaseDirectory
        ORDER BY NombreEmpresa";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            companies.Add(new ContpaqiCompanyDto
            {
                IdDatabase = reader.GetInt64(reader.GetOrdinal("idDataBase")),
                GuidCompany = reader.GetGuid(reader.GetOrdinal("GuidCompany")),
                Version = reader["Version"] as string,
                NombreEmpresa = reader["NombreEmpresa"] as string,
                Alias = reader["Alias"] as string,
                Rfc = reader["RFC"] as string,
                CompanyPath = reader["CompanyPath"] as string,
                DatabaseDocumentsMetadata = reader["DB_DocumentsMetadata"] as string,
                DatabaseDocumentsContent = reader["DB_DocumentsContent"] as string
            });
        }

        return companies;
    }
    private readonly string _connectionString;

    public ContpaqiSqlService(
        IOptions<ContpaqiSqlSettings> settings)
    {
        _connectionString = settings.Value.ConnectionString;
    }

    public async Task<List<string>> GetDatabasesAsync()
    {
        var databases = new List<string>();

        using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        const string sql = @"
            SELECT name
            FROM sys.databases
            ORDER BY name";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            databases.Add(reader.GetString(0));
        }

        return databases;
    }

    public async Task<List<Dictionary<string, object?>>> GetTableDataAsync(
    string databaseName,
    string tableName,
    int top = 100)
    {
        var result = new List<Dictionary<string, object?>>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        var sql = $@"
        SELECT TOP ({top}) *
        FROM [{tableName}]";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] =
                    await reader.IsDBNullAsync(i)
                        ? null
                        : reader.GetValue(i);
            }

            result.Add(row);
        }

        return result;
    }

    public async Task<List<object>> GetColumnsAsync(
    string databaseName,
    string tableName)
    {
        var columns = new List<object>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            COLUMN_NAME,
            DATA_TYPE,
            IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = @tableName
        ORDER BY ORDINAL_POSITION";

        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@tableName", tableName);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            columns.Add(new
            {
                ColumnName = reader["COLUMN_NAME"],
                DataType = reader["DATA_TYPE"],
                IsNullable = reader["IS_NULLABLE"]
            });
        }

        return columns;
    }
    public async Task<List<string>> GetTablesAsync(string databaseName)
    {
        var tables = new List<string>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT TABLE_NAME
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_TYPE = 'BASE TABLE'
        ORDER BY TABLE_NAME";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }
}

