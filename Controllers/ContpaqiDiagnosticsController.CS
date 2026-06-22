using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Integrations.Contpaqi.Services;

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
}