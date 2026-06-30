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

        var companiesResponse = await _supabase.Client
    .From<TicketCompany>()
    .Range(0, 5000)
    .Get();
        var companiesByCustomerId = companiesResponse.Models
    .Where(x => x.ContpaqiCustomerId.HasValue)
    .GroupBy(x => x.ContpaqiCustomerId!.Value)
    .ToDictionary(x => x.Key, x => x.First());

        var contactsResponse = await _supabase.Client
    .From<Contact>()
    .Range(0, 5000)
    .Get();

        var contactsByEmail = contactsResponse.Models
            .Where(x => !string.IsNullOrWhiteSpace(x.Email))
            .GroupBy(x => Normalize(x.Email))
            .ToDictionary(x => x.Key, x => x.First());

        var contactsByContpaqi = contactsResponse.Models
            .Where(x =>
                x.ContpaqiDatabase == databaseName &&
                x.ContpaqiCustomerId.HasValue)
            .GroupBy(x => x.ContpaqiCustomerId!.Value)
            .ToDictionary(x => x.Key, x => x.First());

        var relationsResponse = await _supabase.Client
    .From<ContactCompany>()
    .Range(0, 5000)
    .Get();

        var relationsByContactCompany = relationsResponse.Models
            .GroupBy(x => $"{x.ContactId}-{x.CompanyId}")
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var customer in customers)
        {
            try
            {
                if (!companiesByCustomerId.TryGetValue(customer.Id, out var company))
                {
                    result.Skipped++;
                    continue;
                }

                var primaryName = string.IsNullOrWhiteSpace(customer.RazonSocial)
    ? customer.Codigo
    : customer.RazonSocial;

                var emails = SplitEmails(customer.Email)
                    .Concat(SplitEmails(customer.Email2))
                    .Concat(SplitEmails(customer.Email3))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var primaryEmail = emails.ElementAtOrDefault(0);
                var email2 = emails.ElementAtOrDefault(1);
                var email3 = emails.ElementAtOrDefault(2);

                var contact = await CreateOrUpdateContactAsync(
                    result,
                    contactsByEmail,
                    contactsByContpaqi,
                    databaseName,
                    customer.Id,
                    primaryName,
                    primaryEmail,
                    NormalizePhone(customer.Whatsapp),
                    customer.Estatus == 1,
                    primaryEmail,
                    email2,
                    email3
                );

                await CreateOrUpdateRelationAsync(
                    result,
                    relationsByContactCompany,
                    contact.Id,
                    company.Id,
                    true
                );
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("contact_companies_one_primary_per_company"))
                {
                    result.Skipped++;
                    continue;
                }

                result.Errors.Add($"{customer.Codigo} - {customer.RazonSocial}: {ex.Message}");
            }
        }

        var additionalContacts = await _contpaqiSqlService.GetAdditionalContactsAsync(databaseName);
        result.TotalRead += additionalContacts.Count;

        foreach (var additional in additionalContacts)
        {
            try
            {

                if (!companiesByCustomerId.TryGetValue(additional.CustomerId, out var company))
                {
                    var companyResponse = await _supabase.Client
                        .From<TicketCompany>()
                        .Where(x => x.ContpaqiCustomerId == additional.CustomerId)
                        .Get();

                    company = companyResponse.Models.FirstOrDefault();

                    if (company is null)
                    {

                        result.Skipped++;
                        continue;
                    }

                    companiesByCustomerId[additional.CustomerId] = company;
                }

                var name = string.IsNullOrWhiteSpace(additional.Nombre)
                    ? $"Contacto CONTPAQi {additional.Id}"
                    : additional.Nombre;

                var phone = NormalizePhone(
    !string.IsNullOrWhiteSpace(additional.Telefono1)
        ? additional.Telefono1
        : additional.Telefono2
);

                var contact = await CreateOrUpdateAdditionalContactAsync(
     result,
     databaseName,
     additional.CustomerId,
     additional.Id,
     name,
     additional.Email,
     phone,
     additional.Active
 );

                await CreateOrUpdateRelationAsync(
                    result,
                    relationsByContactCompany,
                    contact.Id,
                    company.Id,
                    false
                );
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Contacto adicional {additional.Id}: {ex.Message}");
            }
        }

        return result;
    }

    private async Task<Contact> CreateOrUpdateContactAsync(
    SyncResultDto result,
    Dictionary<string, Contact> contactsByEmail,
    Dictionary<int, Contact> contactsByContpaqi,
    string databaseName,
    int? contpaqiCustomerId,
    string fullName,
    string? email,
    string? phone,
    bool active,
    string? email1,
    string? email2,
    string? email3)
    {
        Contact? contact = null;

        var normalizedEmail = Normalize(email);

        if (!string.IsNullOrWhiteSpace(normalizedEmail))
        {
            contactsByEmail.TryGetValue(normalizedEmail, out contact);
        }

        if (contact is null && contpaqiCustomerId.HasValue)
        {
            contactsByContpaqi.TryGetValue(contpaqiCustomerId.Value, out contact);
        }
        if (contact is not null && contpaqiCustomerId.HasValue)
        {
            var existingByContpaqiResponse = await _supabase.Client
                .From<Contact>()
                .Where(x => x.ContpaqiDatabase == databaseName)
                .Where(x => x.ContpaqiCustomerId == contpaqiCustomerId.Value)
                .Get();

            var existingByContpaqi = existingByContpaqiResponse.Models.FirstOrDefault();

            if (existingByContpaqi is not null && existingByContpaqi.Id != contact.Id)
            {
                contact = existingByContpaqi;

                contactsByContpaqi[contpaqiCustomerId.Value] = existingByContpaqi;

                if (!string.IsNullOrWhiteSpace(existingByContpaqi.Email))
                {
                    contactsByEmail[Normalize(existingByContpaqi.Email)] = existingByContpaqi;
                }
            }
        }

        if (contact is null && contpaqiCustomerId.HasValue)
        {
            var contactByContpaqiResponse = await _supabase.Client
                .From<Contact>()
                .Where(x => x.ContpaqiDatabase == databaseName)
                .Where(x => x.ContpaqiCustomerId == contpaqiCustomerId.Value)
                .Get();

            contact = contactByContpaqiResponse.Models.FirstOrDefault();

            if (contact is not null)
            {
                contactsByContpaqi[contpaqiCustomerId.Value] = contact;

                if (!string.IsNullOrWhiteSpace(contact.Email))
                {
                    contactsByEmail[Normalize(contact.Email)] = contact;
                }
            }
        }

        if (contact is null)
        {
            var newContact = new Contact
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Email1 = email1,
                Email2 = email2,
                Email3 = email3,
                Phone = phone,
                Active = active,
                ContpaqiCustomerId = contpaqiCustomerId,
                ContpaqiDatabase = databaseName,
                LastSyncedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _supabase.Client
    .From<Contact>()
    .Insert(newContact);

            Contact? savedContact = null;

            if (!string.IsNullOrWhiteSpace(normalizedEmail))
            {
                var savedByEmailResponse = await _supabase.Client
                    .From<Contact>()
                    .Where(x => x.Email == email)
                    .Get();

                savedContact = savedByEmailResponse.Models.FirstOrDefault();
            }

            if (savedContact is null && contpaqiCustomerId.HasValue)
            {
                var savedByContpaqiResponse = await _supabase.Client
                    .From<Contact>()
                    .Where(x => x.ContpaqiDatabase == databaseName)
                    .Where(x => x.ContpaqiCustomerId == contpaqiCustomerId.Value)
                    .Get();

                savedContact = savedByContpaqiResponse.Models.FirstOrDefault();
            }

            if (savedContact is null)
            {
                throw new Exception("No se pudo recuperar el contacto después de insertarlo.");
            }

            if (!string.IsNullOrWhiteSpace(savedContact.Email))
            {
                contactsByEmail[Normalize(savedContact.Email)] = savedContact;
            }

            if (savedContact.ContpaqiCustomerId.HasValue)
            {
                contactsByContpaqi[savedContact.ContpaqiCustomerId.Value] = savedContact;
            }

            result.Created++;

            return savedContact;
        }

        var newEmail = string.IsNullOrWhiteSpace(contact.Email) ? email : contact.Email;
        var newPhone = string.IsNullOrWhiteSpace(contact.Phone) ? phone : contact.Phone;

        var hasChanges =
    contact.FullName != fullName ||
    contact.Email != newEmail ||
    contact.Email1 != email1 ||
    contact.Email2 != email2 ||
    contact.Email3 != email3 ||
    contact.Phone != newPhone ||
    contact.Active != active;

        if (!hasChanges)
        {
            result.Skipped++;
            return contact;
        }

        contact.FullName = fullName;
        contact.Email = newEmail;
        contact.Email1 = email1;
        contact.Email2 = email2;
        contact.Email3 = email3;
        contact.Phone = newPhone;
        contact.Active = active;
        //contact.ContpaqiCustomerId = contpaqiCustomerId;
        //contact.ContpaqiDatabase = databaseName;
        contact.LastSyncedAt = DateTime.UtcNow;
        contact.UpdatedAt = DateTime.UtcNow;

        await contact.Update<Contact>();

        if (!string.IsNullOrWhiteSpace(normalizedEmail))
        {
            contactsByEmail[normalizedEmail] = contact;
        }

        if (contpaqiCustomerId.HasValue)
        {
            contactsByContpaqi[contpaqiCustomerId.Value] = contact;
        }
        result.Updated++;

        return contact;
    }

    private async Task<Contact> CreateOrUpdateAdditionalContactAsync(
    SyncResultDto result,
    string databaseName,
    int contpaqiCustomerId,
    int contpaqiAddressId,
    string fullName,
    string? email,
    string? phone,
    bool active)
    {

        var existingResponse = await _supabase.Client
            .From<Contact>()
            .Where(x => x.ContpaqiDatabase == databaseName)
            .Where(x => x.ContpaqiAddressId == contpaqiAddressId)
            .Get();

        var contact = existingResponse.Models.FirstOrDefault();

        if (contact is null)
        {


            contact = new Contact
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Email1 = email,
                Phone = phone,
                Active = active,
                ContpaqiCustomerId = contpaqiCustomerId,
                ContpaqiAddressId = contpaqiAddressId,
                ContpaqiDatabase = databaseName,
                LastSyncedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _supabase.Client
    .From<Contact>()
    .Insert(contact);

            var savedResponse = await _supabase.Client
                .From<Contact>()
                .Where(x => x.ContpaqiDatabase == databaseName)
                .Where(x => x.ContpaqiAddressId == contpaqiAddressId)
                .Get();

            var savedContact = savedResponse.Models.FirstOrDefault();

            if (savedContact is null)
            {
                throw new Exception($"No se pudo recuperar contacto adicional {contpaqiAddressId} después de insertarlo.");
            }

            result.Created++;
            return savedContact;
        }

        var hasChanges =
            contact.FullName != fullName ||
            contact.Email != email ||
            contact.Email1 != email ||
            contact.Phone != phone ||
            contact.Active != active ||
            contact.ContpaqiCustomerId != contpaqiCustomerId;

        if (!hasChanges)
        {
            result.Skipped++;
            return contact;
        }

        contact.FullName = fullName;
        contact.Email = email;
        contact.Email1 = email;
        contact.Phone = phone;
        contact.Active = active;
        contact.ContpaqiCustomerId = contpaqiCustomerId;
        contact.ContpaqiAddressId = contpaqiAddressId;
        contact.ContpaqiDatabase = databaseName;
        contact.LastSyncedAt = DateTime.UtcNow;
        contact.UpdatedAt = DateTime.UtcNow;

        await contact.Update<Contact>();

        result.Updated++;
        return contact;
    }

    private async Task CreateOrUpdateRelationAsync(
    SyncResultDto result,
    Dictionary<string, ContactCompany> relationsByContactCompany,
    Guid contactId,
    Guid companyId,
    bool isPrimary)
    {
        var key = $"{contactId}-{companyId}";

        ContactCompany? relation = null;

        if (relationsByContactCompany.TryGetValue(key, out var relationFromMemory))
        {
            relation = relationFromMemory;
        }
        else
        {
            var relationResponse = await _supabase.Client
                .From<ContactCompany>()
                .Where(x => x.ContactId == contactId)
                .Where(x => x.CompanyId == companyId)
                .Get();

            relation = relationResponse.Models.FirstOrDefault();

            if (relation is not null)
            {
                relationsByContactCompany[key] = relation;
            }
        }

        if (relation is null)
        {
            if (isPrimary)
            {
                var existingPrimaryResponse = await _supabase.Client
                    .From<ContactCompany>()
                    .Where(x => x.CompanyId == companyId)
                    .Get();

                var existingPrimary = existingPrimaryResponse.Models
                    .FirstOrDefault(x => x.IsPrimary);

                if (existingPrimary is not null)
                {
                    relationsByContactCompany[$"{existingPrimary.ContactId}-{existingPrimary.CompanyId}"] = existingPrimary;
                    result.Skipped++;
                    return;
                }
            }

            var newRelation = new ContactCompany
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                CompanyId = companyId,
                IsPrimary = isPrimary,
                Active = true,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _supabase.Client
                    .From<ContactCompany>()
                    .Insert(newRelation);

                relationsByContactCompany[key] = newRelation;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("contact_companies_one_primary_per_company"))
                {
                    result.Skipped++;
                    return;
                }

                throw;
            }

            return;
        }

        var newIsPrimary = relation.IsPrimary || isPrimary;

        var hasChanges =
            relation.IsPrimary != newIsPrimary ||
            relation.Active != true;

        if (!hasChanges)
        {
            return;
        }

        relation.IsPrimary = newIsPrimary;
        relation.Active = true;
        relation.UpdatedAt = DateTime.UtcNow;

        await relation.Update<ContactCompany>();

        relationsByContactCompany[key] = relation;
    }

    private static string Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();
    }

    private static List<string> SplitEmails(string? emails)
    {
        if (string.IsNullOrWhiteSpace(emails))
        {
            return new List<string>();
        }

        return emails
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
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