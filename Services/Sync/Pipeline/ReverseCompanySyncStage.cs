using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;
using SoporteMida.Api.Services.Sync.Pipeline.Base;

namespace SoporteMida.Api.Services.Sync.Pipeline;

public class ReverseCompanySyncStage : SyncStageBase
{
    private readonly ReverseCompanySyncService _reverseSync;

    public override string Name => "MIDA -> CONTPAQi Empresas";

    public ReverseCompanySyncStage(
        ReverseCompanySyncService reverseSync,
        ILogger<ReverseCompanySyncStage> logger)
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