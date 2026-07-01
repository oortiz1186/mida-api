using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Integrations.Contpaqi.Services;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/contpaqi/reverse-sync")]
public class ContpaqiReverseSyncController : ControllerBase
{
    private readonly ContpaqiReverseSyncService _reverseSyncService;

    public ContpaqiReverseSyncController(
        ContpaqiReverseSyncService reverseSyncService)
    {
        _reverseSyncService = reverseSyncService;
    }

    [HttpPost("companies")]
    public async Task<IActionResult> SyncCompaniesToContpaqi()
    {
        var result = await _reverseSyncService.SyncCompaniesToContpaqiAsync();

        return Ok(result);
    }
}