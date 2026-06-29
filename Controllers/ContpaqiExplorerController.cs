using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Integrations.Contpaqi.Services;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/contpaqi/explorer")]
public class ContpaqiExplorerController : ControllerBase
{
    private readonly ContpaqiSqlService _contpaqiSqlService;

    public ContpaqiExplorerController(ContpaqiSqlService contpaqiSqlService)
    {
        _contpaqiSqlService = contpaqiSqlService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string databaseName,
        [FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
            return BadRequest("databaseName es requerido.");

        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("term es requerido.");

        var results = await _contpaqiSqlService.SearchValueInDatabaseAsync(
    databaseName,
    term
);

        return Ok(results);
    }

    [HttpGet("search-number")]
    public async Task<IActionResult> SearchNumber(
    [FromQuery] string databaseName,
    [FromQuery] long value)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
            return BadRequest("databaseName es requerido.");

        var results = await _contpaqiSqlService.SearchNumberInDatabaseAsync(
            databaseName,
            value
        );

        return Ok(results);
    }
}