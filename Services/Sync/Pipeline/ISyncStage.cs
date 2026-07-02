namespace SoporteMida.Api.Services.Sync.Pipeline;

public interface ISyncStage
{
    string Name { get; }

    Task ExecuteAsync(string databaseName, CancellationToken cancellationToken);
}