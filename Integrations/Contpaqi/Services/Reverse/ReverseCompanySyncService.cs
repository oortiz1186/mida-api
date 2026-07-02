using SoporteMida.Api.Models;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync;

namespace SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;

public class ReverseCompanySyncService
{
    private readonly SupabaseClientService _supabase;
    private readonly ContpaqiSqlService _contpaqiSqlService;

    public ReverseCompanySyncService(
        SupabaseClientService supabase,
        ContpaqiSqlService contpaqiSqlService)
    {
        _supabase = supabase;
        _contpaqiSqlService = contpaqiSqlService;
    }

    public async Task<SyncResultDto> SyncAsync()
    {
        var result = new SyncResultDto();

        var response = await _supabase.Client
            .From<TicketCompany>()
            .Where(x => x.SyncSource == "mida")
            .Where(x => x.SyncStatus == "pending")
            .Range(0, 500)
            .Get();

        result.TotalRead = response.Models.Count;

        foreach (var company in response.Models)
        {
            try
            {
                if (!company.ContpaqiCustomerId.HasValue)
                {
                    SyncMetadataService.MarkAsError(company, "La empresa no tiene contpaqi_customer_id.");
                    await company.Update<TicketCompany>();
                    result.Errors.Add($"{company.Name}: sin contpaqi_customer_id");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(company.ContpaqiDatabase))
                {
                    SyncMetadataService.MarkAsError(company, "La empresa no tiene contpaqi_database.");
                    await company.Update<TicketCompany>();
                    result.Errors.Add($"{company.Name}: sin contpaqi_database");
                    continue;
                }

                await _contpaqiSqlService.UpdateCustomerFromMidaAsync(
                    company.ContpaqiDatabase,
                    company.ContpaqiCustomerId.Value,
                    company.Name,
                    company.Rfc,
                    company.Email,
                    NormalizePhone(company.Phone),
                    company.Active
                );

                SyncMetadataService.MarkAsSyncedFromContpaqi(company);
                await company.Update<TicketCompany>();

                result.Updated++;
            }
            catch (Exception ex)
            {
                SyncMetadataService.MarkAsError(company, ex.Message);
                await company.Update<TicketCompany>();
                result.Errors.Add($"{company.Name}: {ex.Message}");
            }
        }

        return result;
    }

    private static string? NormalizePhone(string? phone)
    {
        return string.IsNullOrWhiteSpace(phone)
            ? null
            : new string(phone.Where(char.IsDigit).ToArray());
    }
}