using SoporteMida.Api.Dtos;
using SoporteMida.Api.Models;

namespace SoporteMida.Api.Services;

public class CompanyService
{
    private readonly SupabaseClientService _supabase;

    public CompanyService(SupabaseClientService supabase)
    {
        _supabase = supabase;
    }

    public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync()
    {
        var response = await _supabase.Client
            .From<Company>()
            .Get();

        return response.Models.Select(company => new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Rfc = company.Rfc,
            Phone = company.Phone,
            Email = company.Email,
            CreatedAt = company.CreatedAt
        });
    }
}
