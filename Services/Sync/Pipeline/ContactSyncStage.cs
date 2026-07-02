using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync.Pipeline.Base;

namespace SoporteMida.Api.Services.Sync.Pipeline;

public class ContactSyncStage : SyncStageBase
{
    private readonly ContpaqiContactSyncService _contactSync;

    public override string Name => "CONTPAQi -> MIDA Contactos";

    public ContactSyncStage(
        ContpaqiContactSyncService contactSync,
        ILogger<ContactSyncStage> logger)
        : base(logger)
    {
        _contactSync = contactSync;
    }

    protected override Task<SyncResultDto> ExecuteStageAsync(
        string databaseName,
        CancellationToken cancellationToken)
    {
        return _contactSync.SyncContactsAsync(databaseName);
    }
}