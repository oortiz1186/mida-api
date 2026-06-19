using Microsoft.AspNetCore.Mvc;
using SoporteMida.Api.Dtos;
using SoporteMida.Api.Models;
using SoporteMida.Api.Services;

namespace SoporteMida.Api.Controllers;

[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly SupabaseClientService _supabase;

    public CompaniesController(SupabaseClientService supabase)
    {
        _supabase = supabase;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var response = await _supabase.Client
            .From<Company>()
            .Get();

        var companies = response.Models.Select(company => new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Rfc = company.Rfc,
            Phone = company.Phone,
            Email = company.Email,
            CreatedAt = company.CreatedAt
        });

        return Ok(companies);
    }
}