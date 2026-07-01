using SoporteMida.Api.Services;
using Microsoft.Extensions.Options;
using SoporteMida.Api.Configuration;
using SoporteMida.Api.Integrations.Contpaqi.Services;

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

                var customerSync = scope.ServiceProvider.GetRequiredService<ContpaqiCustomerSyncService>();
                var contactSync = scope.ServiceProvider.GetRequiredService<ContpaqiContactSyncService>();
                var agentSync = scope.ServiceProvider.GetRequiredService<ContpaqiAgentSyncService>();
                var reverseSync = scope.ServiceProvider.GetRequiredService<ContpaqiReverseSyncService>();


                // Cambia aquí el nombre de la BD real de CONTPAQi
                var databaseName = "adMIDA_PRUEBAS";

                _logger.LogInformation("Iniciando sincronización CONTPAQi para BD: {Database}", databaseName);

                var customersResult = await customerSync.SyncCustomersAsync(databaseName);
                _logger.LogInformation(
    "Empresas sincronizadas. Leídas: {Total}, Creadas: {Created}, Actualizadas: {Updated}, Omitidas: {Skipped}, Errores: {Errors}",
    customersResult.TotalRead,
    customersResult.Created,
    customersResult.Updated,
    customersResult.Skipped,
    customersResult.Errors.Count
);

                var contactsResult = await contactSync.SyncContactsAsync(databaseName);
                _logger.LogInformation(
                    "Contactos sincronizados. Leídos: {Total}, Creados: {Created}, Actualizados: {Updated}, Omitidos: {Skipped}, Errores: {Errors}",
                    contactsResult.TotalRead,
                    contactsResult.Created,
                    contactsResult.Updated,
                    contactsResult.Skipped,
                    contactsResult.Errors.Count
                );

                if (contactsResult.Errors.Any())
                {
                    _logger.LogWarning("Errores en contactos:");

                    foreach (var error in contactsResult.Errors.Take(20))
                    {
                        _logger.LogWarning(error);
                    }
                }

                var agentsResult = await agentSync.SyncAgentsAsync(databaseName);

                _logger.LogInformation(
                    "Asesores sincronizados. Leídas: {Total}, Creadas: {Created}, Actualizadas: {Updated}, Omitidos: {Skipped}, Errores: {Errors}",
                    agentsResult.TotalRead,
                    agentsResult.Created,
                    agentsResult.Updated,
                    agentsResult.Skipped,
                    agentsResult.Errors.Count
                );
                var reverseResult = await reverseSync.SyncCompaniesToContpaqiAsync();

                _logger.LogInformation(
                    "Reverse Sync empresas MIDA -> CONTPAQi. Leídas: {Total}, Actualizadas: {Updated}, Omitidas: {Skipped}, Errores: {Errors}",
                    reverseResult.TotalRead,
                    reverseResult.Updated,
                    reverseResult.Skipped,
                    reverseResult.Errors.Count
                );

                if (reverseResult.Errors.Any())
                {
                    _logger.LogWarning("Errores en Reverse Sync:");

                    foreach (var error in reverseResult.Errors.Take(20))
                    {
                        _logger.LogWarning(error);
                    }
                }

                

                _logger.LogInformation("Sincronización CONTPAQi finalizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general durante la sincronización CONTPAQi.");
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