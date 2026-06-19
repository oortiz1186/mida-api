using Microsoft.AspNetCore.Mvc;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            app = "Soporte MIDA API",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            date = DateTime.UtcNow
        });
    }
}