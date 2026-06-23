using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/companies")]
[Tags("API-SoporteMida")]
public class CompaniesController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompaniesController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    [SwaggerOperation(
    Summary = "Obtener clientes",
    Description = "Devuelve el listado de clientes registrados en Soporte MIDA"
)]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await _companyService.GetCompaniesAsync();
        return Ok(companies);
    }
}
