using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SoporteMida.Api.Config;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;

namespace SoporteMida.Api.Integrations.Contpaqi.Services;

public class ContpaqiSqlService
{

    public async Task<List<ContpaqiDocumentMovementDto>> GetDocumentMovementsAsync(
    string databaseName,
    int documentId)
    {
        var movements = new List<ContpaqiDocumentMovementDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            CIDMOVIMIENTO,
            CIDDOCUMENTO,
            CNUMEROMOVIMIENTO,
            CIDPRODUCTO,
            CUNIDADES,
            CPRECIO,
            CNETO,
            CIMPUESTO1,
            CTOTAL,
            CREFERENCIA,
            CFECHA
        FROM admMovimientos
        WHERE CIDDOCUMENTO = @documentId
        ORDER BY CNUMEROMOVIMIENTO";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@documentId", documentId);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            movements.Add(new ContpaqiDocumentMovementDto
            {
                Id = Convert.ToInt32(reader["CIDMOVIMIENTO"]),
                DocumentId = Convert.ToInt32(reader["CIDDOCUMENTO"]),
                NumeroMovimiento = Convert.ToDouble(reader["CNUMEROMOVIMIENTO"]),
                ProductoId = Convert.ToInt32(reader["CIDPRODUCTO"]),
                Unidades = Convert.ToDouble(reader["CUNIDADES"]),
                Precio = Convert.ToDouble(reader["CPRECIO"]),
                Neto = Convert.ToDouble(reader["CNETO"]),
                Impuesto1 = Convert.ToDouble(reader["CIMPUESTO1"]),
                Total = Convert.ToDouble(reader["CTOTAL"]),
                Referencia = reader["CREFERENCIA"]?.ToString()?.Trim() ?? "",
                Fecha = Convert.ToDateTime(reader["CFECHA"])
            });
        }

        return movements;
    }

    public async Task<List<ContpaqiAgentDto>> GetAgentsAsync(string databaseName)
    {
        var agents = new List<ContpaqiAgentDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            CIDAGENTE,
            CCODIGOAGENTE,
            CNOMBREAGENTE,
            CTIPOAGENTE,
            CFECHAALTAAGENTE
        FROM admAgentes
        WHERE CIDAGENTE > 0
        ORDER BY CNOMBREAGENTE";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            agents.Add(new ContpaqiAgentDto
            {
                Id = Convert.ToInt32(reader["CIDAGENTE"]),
                Codigo = reader["CCODIGOAGENTE"]?.ToString()?.Trim() ?? "",
                Nombre = reader["CNOMBREAGENTE"]?.ToString()?.Trim() ?? "",
                Tipo = Convert.ToInt32(reader["CTIPOAGENTE"]),
                FechaAlta = Convert.ToDateTime(reader["CFECHAALTAAGENTE"])
            });
        }

        return agents;
    }

    public async Task<List<ContpaqiDocumentDto>> GetDocumentsAsync(string databaseName)
    {
        var documents = new List<ContpaqiDocumentDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT TOP (100)
            CIDDOCUMENTO,
            CIDCONCEPTODOCUMENTO,
            CSERIEDOCUMENTO,
            CFOLIO,
            CFECHA,
            CIDCLIENTEPROVEEDOR,
            CRAZONSOCIAL,
            CRFC,
            CNETO,
            CIMPUESTO1,
            CTOTAL,
            CPENDIENTE,
            CCANCELADO,
            CREFERENCIA
        FROM admDocumentos
        ORDER BY CFECHA DESC, CIDDOCUMENTO DESC";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            documents.Add(new ContpaqiDocumentDto
            {
                Id = Convert.ToInt32(reader["CIDDOCUMENTO"]),
                ConceptoId = Convert.ToInt32(reader["CIDCONCEPTODOCUMENTO"]),
                Serie = reader["CSERIEDOCUMENTO"]?.ToString()?.Trim() ?? "",
                Folio = Convert.ToDouble(reader["CFOLIO"]),
                Fecha = Convert.ToDateTime(reader["CFECHA"]),
                ClienteId = Convert.ToInt32(reader["CIDCLIENTEPROVEEDOR"]),
                RazonSocial = reader["CRAZONSOCIAL"]?.ToString()?.Trim() ?? "",
                Rfc = reader["CRFC"]?.ToString()?.Trim() ?? "",
                Neto = Convert.ToDouble(reader["CNETO"]),
                Impuesto1 = Convert.ToDouble(reader["CIMPUESTO1"]),
                Total = Convert.ToDouble(reader["CTOTAL"]),
                Pendiente = Convert.ToDouble(reader["CPENDIENTE"]),
                Cancelado = Convert.ToInt32(reader["CCANCELADO"]),
                Referencia = reader["CREFERENCIA"]?.ToString()?.Trim() ?? ""
            });
        }

        return documents;
    }

    public async Task<List<ContpaqiProductDto>> GetProductsAsync(string databaseName)
    {
        var products = new List<ContpaqiProductDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            CIDPRODUCTO,
            CCODIGOPRODUCTO,
            CNOMBREPRODUCTO,
            CTIPOPRODUCTO,
            CSTATUSPRODUCTO
        FROM admProductos
        WHERE CIDPRODUCTO > 0
        ORDER BY CNOMBREPRODUCTO";

        using var command = new SqlCommand(sql, connection);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new ContpaqiProductDto
            {
                Id = Convert.ToInt32(reader["CIDPRODUCTO"]),
                Codigo = reader["CCODIGOPRODUCTO"]?.ToString()?.Trim() ?? "",
                Nombre = reader["CNOMBREPRODUCTO"]?.ToString()?.Trim() ?? "",
                TipoProducto = reader["CTIPOPRODUCTO"]?.ToString()?.Trim(),
                Estatus = Convert.ToInt32(reader["CSTATUSPRODUCTO"])
            });
        }

        return products;
    }

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
    c.CIDCLIENTEPROVEEDOR,
    c.CCODIGOCLIENTE,
    c.CRAZONSOCIAL,
    c.CRFC,
    COALESCE(NULLIF(c.CEMAIL1, ''), NULLIF(d.CEMAIL, '')) AS CEMAIL1,
    c.CEMAIL2,
    c.CEMAIL3,
    c.CUSOCFDI,
    c.CREGIMFISC,
    COALESCE(NULLIF(c.CWHATSAPP, ''), NULLIF(d.CTELEFONO1, ''), NULLIF(d.CTELEFONO2, '')) AS CWHATSAPP,
    c.CESTATUS
FROM admClientes c
OUTER APPLY (
    SELECT TOP 1
        CEMAIL,
        CTELEFONO1,
        CTELEFONO2
    FROM admDomicilios d
    WHERE d.CIDCATALOGO = c.CIDCLIENTEPROVEEDOR
      AND d.CTIPOCATALOGO = 1
    ORDER BY
        CASE
            WHEN d.CSUCURSAL = 'Matriz' THEN 0
            WHEN d.CTIPODIRECCION = 0 THEN 1
            ELSE 2
        END,
        d.CIDDIRECCION
) d
WHERE c.CIDCLIENTEPROVEEDOR > 0
ORDER BY c.CRAZONSOCIAL";

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
                Email2 = reader["CEMAIL2"]?.ToString()?.Trim(),
                Email3 = reader["CEMAIL3"]?.ToString()?.Trim(),
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
    public async Task<List<object>> SearchColumnsAsync(
    string databaseName,
    string term)
    {
        var result = new List<object>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        const string sql = @"
        SELECT
            TABLE_NAME,
            COLUMN_NAME,
            DATA_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE
            COLUMN_NAME LIKE @term
            OR TABLE_NAME LIKE @term
        ORDER BY TABLE_NAME, COLUMN_NAME";

        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@term", $"%{term}%");

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new
            {
                TableName = reader["TABLE_NAME"]?.ToString(),
                ColumnName = reader["COLUMN_NAME"]?.ToString(),
                DataType = reader["DATA_TYPE"]?.ToString()
            });
        }

        return result;
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
    public async Task<List<object>> SearchValueInDatabaseAsync(
    string databaseName,
    string term)
    {
        var results = new List<object>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        const string columnsSql = @"
        SELECT
            TABLE_NAME,
            COLUMN_NAME,
            DATA_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar', 'text', 'ntext')
        ORDER BY TABLE_NAME, ORDINAL_POSITION";

        using var columnsCommand = new SqlCommand(columnsSql, connection);
        using var reader = await columnsCommand.ExecuteReaderAsync();

        var columns = new List<(string TableName, string ColumnName)>();

        while (await reader.ReadAsync())
        {
            columns.Add((
                reader["TABLE_NAME"]?.ToString() ?? "",
                reader["COLUMN_NAME"]?.ToString() ?? ""
            ));
        }

        await reader.CloseAsync();

        foreach (var group in columns.GroupBy(x => x.TableName))
        {
            var tableName = group.Key;

            foreach (var column in group)
            {
                try
                {
                    var sql = $@"
                    SELECT TOP 20 *
                    FROM [{tableName}]
                    WHERE [{column.ColumnName}] LIKE @term";

                    using var command = new SqlCommand(sql, connection);
                    command.CommandTimeout = 10;
                    command.Parameters.AddWithValue("@term", $"%{term}%");

                    using var dataReader = await command.ExecuteReaderAsync();

                    while (await dataReader.ReadAsync())
                    {
                        var row = new Dictionary<string, object?>();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            row[dataReader.GetName(i)] =
                                await dataReader.IsDBNullAsync(i)
                                    ? null
                                    : dataReader.GetValue(i);
                        }

                        results.Add(new
                        {
                            TableName = tableName,
                            ColumnName = column.ColumnName,
                            Row = row
                        });
                    }

                    await dataReader.CloseAsync();
                }
                catch
                {
                    // Ignorar tablas/columnas que no se puedan leer.
                }
            }
        }

        return results;
    }

    public async Task<List<ContpaqiAdditionalContactDto>> GetAdditionalContactsAsync(string databaseName)
    {
        var contacts = new List<ContpaqiAdditionalContactDto>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        const string sql = @"
        SELECT
            CIDDIRECCION,
            CIDCATALOGO,
            CNOMBRECALLE,
            CEMAIL,
            CTELEFONO1,
            CTELEFONO2
        FROM admDomicilios
        WHERE CTIPOCATALOGO = 6
          AND CIDCATALOGO > 0
        ORDER BY CIDCATALOGO, CIDDIRECCION";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            contacts.Add(new ContpaqiAdditionalContactDto
            {
                Id = Convert.ToInt32(reader["CIDDIRECCION"]),
                CustomerId = Convert.ToInt32(reader["CIDCATALOGO"]),
                Nombre = reader["CNOMBRECALLE"]?.ToString()?.Trim(),
                Email = reader["CEMAIL"]?.ToString()?.Trim(),
                Telefono1 = reader["CTELEFONO1"]?.ToString()?.Trim(),
                Telefono2 = reader["CTELEFONO2"]?.ToString()?.Trim(),
                Active = true
            });
        }

        return contacts;
    }

    public async Task<List<object>> SearchNumberInDatabaseAsync(
    string databaseName,
    long value)
    {
        var results = new List<object>();

        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = databaseName
        };

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        const string columnsSql = @"
        SELECT
            TABLE_NAME,
            COLUMN_NAME,
            DATA_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE DATA_TYPE IN (
            'int',
            'bigint',
            'smallint',
            'tinyint',
            'numeric',
            'decimal',
            'float',
            'real'
        )
        ORDER BY TABLE_NAME, ORDINAL_POSITION";

        using var columnsCommand = new SqlCommand(columnsSql, connection);
        using var reader = await columnsCommand.ExecuteReaderAsync();

        var columns = new List<(string TableName, string ColumnName, string DataType)>();

        while (await reader.ReadAsync())
        {
            columns.Add((
                reader["TABLE_NAME"]?.ToString() ?? "",
                reader["COLUMN_NAME"]?.ToString() ?? "",
                reader["DATA_TYPE"]?.ToString() ?? ""
            ));
        }

        await reader.CloseAsync();

        foreach (var column in columns)
        {
            try
            {
                var sql = $@"
                SELECT COUNT(1)
                FROM [{column.TableName}]
                WHERE [{column.ColumnName}] = @value";

                using var command = new SqlCommand(sql, connection);
                command.CommandTimeout = 10;
                command.Parameters.AddWithValue("@value", value);

                var count = Convert.ToInt32(await command.ExecuteScalarAsync());

                if (count > 0)
                {
                    results.Add(new
                    {
                        TableName = column.TableName,
                        ColumnName = column.ColumnName,
                        DataType = column.DataType,
                        Matches = count
                    });
                }
            }
            catch
            {
                // Ignorar columnas/tablas que no se puedan consultar.
            }
        }

        return results
            .OrderByDescending(x =>
                Convert.ToInt32(
                    x.GetType().GetProperty("Matches")!.GetValue(x)
                )
            )
            .ToList();
    }
}

