using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/contpaqi")]
public class ContpaqiDiagnosticsController : ControllerBase
{
    private readonly ContpaqiSqlService _contpaqiSqlService;

    public ContpaqiDiagnosticsController(
        ContpaqiSqlService contpaqiSqlService)
    {
        _contpaqiSqlService = contpaqiSqlService;
    }

    [HttpGet("databases")]
    public async Task<IActionResult> GetDatabases()
    {
        var databases = await _contpaqiSqlService.GetDatabasesAsync();

        return Ok(databases);
    }

    [HttpGet("tables/{databaseName}")]
    public async Task<IActionResult> GetTables(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return BadRequest("El nombre de la base de datos es requerido.");
        }

        var tables = await _contpaqiSqlService.GetTablesAsync(databaseName);

        return Ok(tables);
    }

    [HttpGet("table/{databaseName}/{tableName}")]
    public async Task<IActionResult> GetTableData(
    string databaseName,
    string tableName)
    {
        var data = await _contpaqiSqlService
            .GetTableDataAsync(databaseName, tableName);

        return Ok(data);
    }

    [HttpGet("columns/{databaseName}/{tableName}")]
    public async Task<IActionResult> GetColumns(
    string databaseName,
    string tableName)
    {
        var columns = await _contpaqiSqlService
            .GetColumnsAsync(databaseName, tableName);

        return Ok(columns);
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await _contpaqiSqlService.GetCompaniesAsync();

        return Ok(companies);
    }

    [HttpGet("customers/{databaseName}")]
    public async Task<IActionResult> GetCustomers(string databaseName)
    {
        var customers = await _contpaqiSqlService.GetCustomersAsync(databaseName);

        return Ok(customers);
    }

    [HttpGet("products/{databaseName}")]
    public async Task<IActionResult> GetProducts(string databaseName)
    {
        var products = await _contpaqiSqlService.GetProductsAsync(databaseName);

        return Ok(products);
    }

    [HttpGet("documents/{databaseName}")]
    public async Task<IActionResult> GetDocuments(string databaseName)
    {
        var documents = await _contpaqiSqlService.GetDocumentsAsync(databaseName);

        return Ok(documents);
    }

    [HttpGet("documents/{databaseName}/{documentId}/movements")]
    public async Task<IActionResult> GetDocumentMovements(
    string databaseName,
    int documentId)
    {
        var movements = await _contpaqiSqlService
            .GetDocumentMovementsAsync(databaseName, documentId);

        return Ok(movements);
    }

    [HttpGet("search-columns/{databaseName}/{term}")]
    [SwaggerOperation(
    Summary = "Buscar columnas o tablas",
    Description = "Busca tablas o columnas dentro de una base CONTPAQi usando una palabra clave."
)]
    public async Task<IActionResult> SearchColumns(
    string databaseName,
    string term)
    {
        var result = await _contpaqiSqlService
            .SearchColumnsAsync(databaseName, term);

        return Ok(result);
    }
}