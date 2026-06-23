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
    private readonly ContpaqiContactSyncService _contactSyncService;
    private readonly ContpaqiAgentSyncService _agentSyncService;

    public SyncController(ContpaqiCustomerSyncService customerSyncService, ContpaqiContactSyncService contactSyncService, ContpaqiAgentSyncService agentSyncService)
    {
        _customerSyncService = customerSyncService;
        _contactSyncService = contactSyncService;
        _agentSyncService = agentSyncService;
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

    [HttpPost("contacts/{databaseName}")]
    [SwaggerOperation(
    Summary = "Sincronizar contactos",
    Description = "Sincroniza contactos desde CONTPAQi hacia contacts y contact_companies en Soporte MIDA."
)]
    public async Task<IActionResult> SyncContacts(string databaseName)
    {
        var result = await _contactSyncService.SyncContactsAsync(databaseName);

        return Ok(result);
    }

    [HttpPost("agents/{databaseName}")]
    [SwaggerOperation(
    Summary = "Sincronizar agentes",
    Description = "Sincroniza agentes desde CONTPAQi hacia advisors en Soporte MIDA."
)]
    public async Task<IActionResult> SyncAgents(string databaseName)
    {
        var result = await _agentSyncService.SyncAgentsAsync(databaseName);

        return Ok(result);
    }
}