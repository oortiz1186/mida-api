namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class FullSyncResultDto
{
    public string Database { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public SyncResultDto Customers { get; set; } = new();
    public SyncResultDto Contacts { get; set; } = new();
    public SyncResultDto Agents { get; set; } = new();

    public bool HasErrors =>
        Customers.Errors.Any() ||
        Contacts.Errors.Any() ||
        Agents.Errors.Any();
}