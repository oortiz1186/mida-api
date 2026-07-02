using SoporteMida.Api.Models;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync;

namespace SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;

public class ReverseContactSyncService
{
    private readonly SupabaseClientService _supabase;
    private readonly ContpaqiSqlService _contpaqiSqlService;

    public ReverseContactSyncService(
        SupabaseClientService supabase,
        ContpaqiSqlService contpaqiSqlService)
    {
        _supabase = supabase;
        _contpaqiSqlService = contpaqiSqlService;
    }

    public async Task<SyncResultDto> SyncAsync()
    {
        var result = new SyncResultDto();

        var contactsResponse = await _supabase.Client
            .From<Contact>()
            .Where(x => x.SyncSource == "mida")
            .Where(x => x.SyncStatus == "pending")
            .Range(0, 500)
            .Get();

        result.TotalRead = contactsResponse.Models.Count;

        foreach (var contact in contactsResponse.Models)
        {
            try
            {
                if (!contact.ContpaqiCustomerId.HasValue)
                {
                    SyncMetadataService.MarkAsError(contact, "El contacto no tiene contpaqi_customer_id.");
                    await contact.Update<Contact>();
                    result.Errors.Add($"{contact.FullName}: sin contpaqi_customer_id");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(contact.ContpaqiDatabase))
                {
                    SyncMetadataService.MarkAsError(contact, "El contacto no tiene contpaqi_database.");
                    await contact.Update<Contact>();
                    result.Errors.Add($"{contact.FullName}: sin contpaqi_database");
                    continue;
                }

                var phone = NormalizePhone(contact.Phone);
                var email = !string.IsNullOrWhiteSpace(contact.Email1)
                    ? contact.Email1
                    : contact.Email;

                if (contact.ContpaqiAddressId.HasValue)
                {
                    await _contpaqiSqlService.UpdateAdditionalContactFromMidaAsync(
                        contact.ContpaqiDatabase,
                        contact.ContpaqiAddressId.Value,
                        contact.ContpaqiCustomerId.Value,
                        contact.FullName,
                        email,
                        phone,
                        contact.Active
                    );

                    result.Updated++;
                }
                else
                {
                    var addressId = await _contpaqiSqlService.InsertAdditionalContactFromMidaAsync(
                        contact.ContpaqiDatabase,
                        contact.ContpaqiCustomerId.Value,
                        contact.FullName,
                        email,
                        phone,
                        contact.Active
                    );

                    contact.ContpaqiAddressId = addressId;
                    result.Created++;
                }

                SyncMetadataService.MarkAsSyncedFromContpaqi(contact);
                await contact.Update<Contact>();
            }
            catch (Exception ex)
            {
                SyncMetadataService.MarkAsError(contact, ex.Message);
                await contact.Update<Contact>();
                result.Errors.Add($"{contact.FullName}: {ex.Message}");
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