using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/sync/contpaqi")]
[Tags("Sincronización CONTPAQi")]
public class SyncController : ControllerBase
{
    private readonly ContpaqiCustomerSyncService _customerSyncService;

    public SyncController(ContpaqiCustomerSyncService customerSyncService)
    {
        _customerSyncService = customerSyncService;
    }

    [HttpPost("customers/{databaseName}")]
    [SwaggerOperation(
        Summary = "Sincronizar clientes",
        Description = "Sincroniza clientes desde CONTPAQi Comercial hacia ticket_companies en Soporte MIDA."
    )]
    public async Task<IActionResult> SyncCustomers(string databaseName)
    {
        var result = await _customerSyncService.SyncCustomersAsync(databaseName);

        return Ok(result);
    }
}