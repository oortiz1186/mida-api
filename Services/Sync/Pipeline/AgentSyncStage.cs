using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync.Pipeline.Base;

namespace SoporteMida.Api.Services.Sync.Pipeline;

public class AgentSyncStage : SyncStageBase
{
    private readonly ContpaqiAgentSyncService _agentSync;

    public override string Name => "CONTPAQi -> MIDA Asesores";

    public AgentSyncStage(
        ContpaqiAgentSyncService agentSync,
        ILogger<AgentSyncStage> logger)
        : base(logger)
    {
        _agentSync = agentSync;
    }

    protected override Task<SyncResultDto> ExecuteStageAsync(
        string databaseName,
        CancellationToken cancellationToken)
    {
        return _agentSync.SyncAgentsAsync(databaseName);
    }
}