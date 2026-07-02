using System.Diagnostics;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;

namespace SoporteMida.Api.Services.Sync.Pipeline.Base;

public abstract class SyncStageBase : ISyncStage
{
    private readonly ILogger _logger;

    public abstract string Name { get; }

    protected SyncStageBase(ILogger logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(string databaseName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Iniciando etapa: {Stage}", Name);

        try
        {
            var result = await ExecuteStageAsync(databaseName, cancellationToken);

            stopwatch.Stop();

            LogResult(result);

            _logger.LogInformation(
                "Etapa finalizada: {Stage}. Tiempo: {Seconds}s",
                Name,
                stopwatch.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error en etapa: {Stage}. Tiempo: {Seconds}s",
                Name,
                stopwatch.Elapsed.TotalSeconds);
        }
    }

    protected abstract Task<SyncResultDto> ExecuteStageAsync(
        string databaseName,
        CancellationToken cancellationToken);

    protected virtual void LogResult(SyncResultDto result)
    {
        _logger.LogInformation(
            "{Stage}. Leídos: {Total}, Creados: {Created}, Actualizados: {Updated}, Omitidos: {Skipped}, Errores: {Errors}",
            Name,
            result.TotalRead,
            result.Created,
            result.Updated,
            result.Skipped,
            result.Errors.Count);

        foreach (var error in result.Errors.Take(20))
        {
            _logger.LogWarning("{Stage} error: {Error}", Name, error);
        }
    }
}