using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync.Pipeline.Base;

namespace SoporteMida.Api.Services.Sync.Pipeline;

public class CompanySyncStage : SyncStageBase
{
    private readonly ContpaqiCustomerSyncService _customerSync;

    public override string Name => "CONTPAQi -> MIDA Empresas";

    public CompanySyncStage(
        ContpaqiCustomerSyncService customerSync,
        ILogger<CompanySyncStage> logger)
        : base(logger)
    {
        _customerSync = customerSync;
    }

    protected override Task<SyncResultDto> ExecuteStageAsync(
        string databaseName,
        CancellationToken cancellationToken)
    {
        return _customerSync.SyncCustomersAsync(databaseName);
    }
}