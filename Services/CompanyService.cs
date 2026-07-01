using SoporteMida.Api.Dtos;
using SoporteMida.Api.Models;
using SoporteMida.Api.Controllers;
using SoporteMida.Api.Services.Sync;

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
    .From<TicketCompany>()
    .Range(0, 5000)
    .Get();

        return response.Models.Select(company => new CompanyDto
        {
            Id = company.Id.ToString(),
            Name = company.Name,
            Rfc = company.Rfc,
            Phone = company.Phone,
            Email = company.Email,
            CreatedAt = company.UpdatedAt
        });
    }

    public async Task<TicketCompany?> UpdateCompanyAsync(Guid id, UpdateCompanyRequest request)
    {
        var response = await _supabase.Client
            .From<TicketCompany>()
            .Where(x => x.Id == id)
            .Get();

        var company = response.Models.FirstOrDefault();

        if (company is null)
        {
            return null;
        }

        company.Name = request.Name;
        company.Rfc = request.Rfc;
        company.Email = request.Email;
        company.Phone = request.Phone;
        company.Active = request.Active;

        SyncMetadataService.MarkAsPendingFromMida(company);

        await company.Update<TicketCompany>();

        return company;
    }
}