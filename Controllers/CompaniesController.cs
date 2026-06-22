using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Services;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly CompanyService _companyService;

    public CompaniesController(CompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await _companyService.GetCompaniesAsync();
        return Ok(companies);
    }
}
