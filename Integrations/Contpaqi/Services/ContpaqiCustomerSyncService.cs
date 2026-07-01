using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Models;
using SoporteMida.Api.Services.Sync;

namespace SoporteMida.Api.Services;

public class ContpaqiCustomerSyncService
{
    private readonly ContpaqiSqlService _contpaqiSqlService;
    private readonly SupabaseClientService _supabase;

    public ContpaqiCustomerSyncService(
        ContpaqiSqlService contpaqiSqlService,
        SupabaseClientService supabase)
    {
        _contpaqiSqlService = contpaqiSqlService;
        _supabase = supabase;
    }

    public async Task<SyncResultDto> SyncCustomersAsync(string databaseName)
    {
        var result = new SyncResultDto();

        var customers = await _contpaqiSqlService.GetCustomersAsync(databaseName);
        result.TotalRead = customers.Count;

        foreach (var customer in customers)
        {
            try
            {
                var existing = await _supabase.Client
                    .From<TicketCompany>()
                    .Where(x => x.ContpaqiDatabase == databaseName)
                    .Where(x => x.ContpaqiCustomerId == customer.Id)
                    .Get();

                var company = existing.Models.FirstOrDefault();

                if (company is null)
                {
                    company = new TicketCompany
                    {
                        Id = Guid.NewGuid(),
                        Name = customer.RazonSocial,
                        Rfc = customer.Rfc,
                        Email = customer.Email,
                        Phone = NormalizePhone(customer.Whatsapp),
                        Active = customer.Estatus == 1,
                        ContpaqiCustomerId = customer.Id,
                        ContpaqiCode = customer.Codigo,
                        ContpaqiDatabase = databaseName,

                        SyncSource = "contpaqi",
                        SyncStatus = "synced",
                        SyncError = null,
                        LastRemoteChangeAt = DateTime.UtcNow,

                        LastSyncedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _supabase.Client
                        .From<TicketCompany>()
                        .Insert(company);

                    result.Created++;
                }
                else
                {
                    if (SyncConflictResolver.ShouldSkipContpaqiUpdate(company))
                    {
                        result.Skipped++;
                        continue;
                    }

                    var hasChanges =
                        company.Name != customer.RazonSocial ||
                        company.Rfc != customer.Rfc ||
                        company.Email != customer.Email ||
                        company.Phone != NormalizePhone(customer.Whatsapp) ||
                        company.Active != (customer.Estatus == 1) ||
                        company.ContpaqiCode != customer.Codigo;

                    if (!hasChanges)
                    {
                        result.Skipped++;
                        continue;
                    }

                   SyncMetadataService.MarkAsSyncedFromContpaqi(company);

                    company.Name = customer.RazonSocial;
                    company.Rfc = customer.Rfc;
                    company.Email = customer.Email;
                    company.Phone = NormalizePhone(customer.Whatsapp);
                    company.Active = customer.Estatus == 1;
                    company.ContpaqiCode = customer.Codigo;

                    
                    await company.Update<TicketCompany>();

                    result.Updated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"{customer.Codigo} - {customer.RazonSocial}: {ex.Message}");
            }
        }

        return result;
    }
    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        return new string(phone.Where(char.IsDigit).ToArray());
    }
}