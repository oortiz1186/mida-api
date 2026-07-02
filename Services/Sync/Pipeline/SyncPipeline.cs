namespace SoporteMida.Api.Services.Sync.Pipeline;

public class SyncPipeline
{
    private readonly IEnumerable<ISyncStage> _stages;

    public SyncPipeline(IEnumerable<ISyncStage> stages)
    {
        _stages = stages;
    }

    public async Task ExecuteAsync(string databaseName, CancellationToken cancellationToken)
    {
        foreach (var stage in _stages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await stage.ExecuteAsync(databaseName, cancellationToken);
        }
    }
}