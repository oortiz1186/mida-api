using Microsoft.Extensions.Options;
using SoporteMida.Api.Configuration;
using SoporteMida.Api.Services.Sync.Pipeline;

namespace SoporteMida.Api.Workers;

public class ContpaqiSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContpaqiSyncWorker> _logger;
    private readonly ContpaqiSyncOptions _syncOptions;
    private bool _isRunning = false;

    public ContpaqiSyncWorker(
        IServiceProvider serviceProvider,
        ILogger<ContpaqiSyncWorker> logger,
        IOptions<ContpaqiSyncOptions> syncOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _syncOptions = syncOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de sincronización CONTPAQi iniciado.");
        _logger.LogInformation("=================================");
        _logger.LogInformation("WORKER INICIADO");
        _logger.LogInformation("=================================");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("");
            _logger.LogInformation("*********************************");
            _logger.LogInformation("SINCRONIZANDO: {Hora}", DateTime.Now);
            _logger.LogInformation("*********************************");
            _logger.LogInformation("");

            if (_isRunning)
            {
                _logger.LogWarning("La sincronización anterior sigue corriendo. Se omite esta vuelta.");

                await Task.Delay(
                    TimeSpan.FromMinutes(_syncOptions.IntervalMinutes),
                    stoppingToken);

                continue;
            }

            try
            {
                _isRunning = true;

                using var scope = _serviceProvider.CreateScope();

                var pipeline = scope.ServiceProvider.GetRequiredService<SyncPipeline>();

                var databaseName = _syncOptions.DatabaseName;

                _logger.LogInformation(
                    "Iniciando SyncPipeline CONTPAQi para BD: {Database}",
                    databaseName);

                await pipeline.ExecuteAsync(databaseName, stoppingToken);

                _logger.LogInformation("SyncPipeline CONTPAQi finalizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general durante SyncPipeline CONTPAQi.");
            }
            finally
            {
                _isRunning = false;
            }

            _logger.LogInformation(
                "Próxima sincronización en {Minutes} minuto(s)...",
                _syncOptions.IntervalMinutes);

            await Task.Delay(
                TimeSpan.FromMinutes(_syncOptions.IntervalMinutes),
                stoppingToken);
        }
    }
}