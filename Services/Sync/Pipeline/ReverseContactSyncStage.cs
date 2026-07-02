using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;
using SoporteMida.Api.Services.Sync.Pipeline.Base;

namespace SoporteMida.Api.Services.Sync.Pipeline;

public class ReverseContactSyncStage : SyncStageBase
{
    private readonly ReverseContactSyncService _reverseSync;

    public override string Name => "MIDA -> CONTPAQi Contactos";

    public ReverseContactSyncStage(
        ReverseContactSyncService reverseSync,
        ILogger<ReverseContactSyncStage> logger)
        : base(logger)
    {
        _reverseSync = reverseSync;
    }

    protected override Task<SyncResultDto> ExecuteStageAsync(
        string databaseName,
        CancellationToken cancellationToken)
    {
        return _reverseSync.SyncAsync();
    }
}