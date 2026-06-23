using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Models;

namespace SoporteMida.Api.Services;

public class ContpaqiContactSyncService
{
    private readonly ContpaqiSqlService _contpaqiSqlService;
    private readonly SupabaseClientService _supabase;

    public ContpaqiContactSyncService(
        ContpaqiSqlService contpaqiSqlService,
        SupabaseClientService supabase)
    {
        _contpaqiSqlService = contpaqiSqlService;
        _supabase = supabase;
    }

    public async Task<SyncResultDto> SyncContactsAsync(string databaseName)
    {
        var result = new SyncResultDto();

        var customers = await _contpaqiSqlService.GetCustomersAsync(databaseName);
        result.TotalRead = customers.Count;

        foreach (var customer in customers)
        {
            try
            {
                var companyResponse = await _supabase.Client
                    .From<TicketCompany>()
                    .Where(x => x.ContpaqiDatabase == databaseName)
                    .Where(x => x.ContpaqiCustomerId == customer.Id)
                    .Get();

                var company = companyResponse.Models.FirstOrDefault();

                if (company is null)
                {
                    result.Skipped++;
                    continue;
                }

                var primaryName = string.IsNullOrWhiteSpace(customer.RazonSocial)
                    ? customer.Codigo
                    : customer.RazonSocial;

                var primaryContact = await CreateOrUpdateContactAsync(
                    databaseName,
                    customer.Id,
                    primaryName,
                    customer.Email,
                    customer.Whatsapp,
                    customer.Estatus == 1
                );

                await CreateOrUpdateRelationAsync(
                    primaryContact.Id,
                    company.Id,
                    true
                );

                if (!string.IsNullOrWhiteSpace(customer.Email2))
                {
                    var contact2 = await CreateOrUpdateContactAsync(
                        databaseName,
                        customer.Id,
                        $"Contacto 2 - {primaryName}",
                        customer.Email2,
                        null,
                        customer.Estatus == 1
                    );

                    await CreateOrUpdateRelationAsync(
                        contact2.Id,
                        company.Id,
                        false
                    );
                }

                if (!string.IsNullOrWhiteSpace(customer.Email3))
                {
                    var contact3 = await CreateOrUpdateContactAsync(
                        databaseName,
                        customer.Id,
                        $"Contacto 3 - {primaryName}",
                        customer.Email3,
                        null,
                        customer.Estatus == 1
                    );

                    await CreateOrUpdateRelationAsync(
                        contact3.Id,
                        company.Id,
                        false
                    );
                }

                result.Updated++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"{customer.Codigo} - {customer.RazonSocial}: {ex.Message}");
            }
        }

        return result;
    }

    private async Task<Contact> CreateOrUpdateContactAsync(
        string databaseName,
        int contpaqiCustomerId,
        string fullName,
        string? email,
        string? phone,
        bool active)
    {
        Contact? contact = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var contactByEmailResponse = await _supabase.Client
                .From<Contact>()
                .Where(x => x.Email == email)
                .Get();

            contact = contactByEmailResponse.Models.FirstOrDefault();
        }

        if (contact is null)
        {
            var contactByContpaqiResponse = await _supabase.Client
                .From<Contact>()
                .Where(x => x.ContpaqiDatabase == databaseName)
                .Where(x => x.ContpaqiCustomerId == contpaqiCustomerId)
                .Get();

            contact = contactByContpaqiResponse.Models.FirstOrDefault();
        }

        if (contact is null)
        {
            contact = new Contact
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Phone = phone,
                Active = active,
                ContpaqiCustomerId = contpaqiCustomerId,
                ContpaqiDatabase = databaseName,
                LastSyncedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _supabase.Client
                .From<Contact>()
                .Insert(contact);

            return contact;
        }

        contact.FullName = fullName;
        contact.Email = string.IsNullOrWhiteSpace(contact.Email) ? email : contact.Email;
        contact.Phone = string.IsNullOrWhiteSpace(contact.Phone) ? phone : contact.Phone;
        contact.Active = active;
        contact.LastSyncedAt = DateTime.UtcNow;
        contact.UpdatedAt = DateTime.UtcNow;

        await contact.Update<Contact>();

        return contact;
    }

    private async Task CreateOrUpdateRelationAsync(
        Guid contactId,
        Guid companyId,
        bool isPrimary)
    {
        var relationResponse = await _supabase.Client
            .From<ContactCompany>()
            .Where(x => x.ContactId == contactId)
            .Where(x => x.CompanyId == companyId)
            .Get();

        var relation = relationResponse.Models.FirstOrDefault();

        if (relation is null)
        {
            relation = new ContactCompany
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                CompanyId = companyId,
                IsPrimary = isPrimary,
                Active = true,
                UpdatedAt = DateTime.UtcNow
            };

            await _supabase.Client
                .From<ContactCompany>()
                .Insert(relation);

            return;
        }

        relation.IsPrimary = relation.IsPrimary || isPrimary;
        relation.Active = true;
        relation.UpdatedAt = DateTime.UtcNow;

        await relation.Update<ContactCompany>();
    }
}