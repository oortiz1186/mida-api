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

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        var company = await _companyService.UpdateCompanyAsync(id, request);

        if (company is null)
        {
            return NotFound("Empresa no encontrada.");
        }

        return Ok(company);
    }
}

public class UpdateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool Active { get; set; } = true;
}