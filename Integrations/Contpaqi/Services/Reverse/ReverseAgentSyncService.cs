using SoporteMida.Api.Models;
using SoporteMida.Api.Services;
using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Services.Sync;

namespace SoporteMida.Api.Integrations.Contpaqi.Services.Reverse;

public class ReverseAgentSyncService
{
    private readonly SupabaseClientService _supabase;
    private readonly ContpaqiSqlService _contpaqiSqlService;

    public ReverseAgentSyncService(
        SupabaseClientService supabase,
        ContpaqiSqlService contpaqiSqlService)
    {
        _supabase = supabase;
        _contpaqiSqlService = contpaqiSqlService;
    }

    public async Task<SyncResultDto> SyncAsync()
    {
        var result = new SyncResultDto();

        var response = await _supabase.Client
            .From<Advisor>()
            .Where(x => x.SyncSource == "mida")
            .Where(x => x.SyncStatus == "pending")
            .Range(0, 500)
            .Get();

        result.TotalRead = response.Models.Count;

        foreach (var advisor in response.Models)
        {
            try
            {
                if (!advisor.ContpaqiAgentId.HasValue)
                {
                    SyncMetadataService.MarkAsError(advisor, "El asesor no tiene contpaqi_agent_id.");
                    await advisor.Update<Advisor>();

                    result.Errors.Add($"{advisor.Name}: sin contpaqi_agent_id");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(advisor.ContpaqiDatabase))
                {
                    SyncMetadataService.MarkAsError(advisor, "El asesor no tiene contpaqi_database.");
                    await advisor.Update<Advisor>();

                    result.Errors.Add($"{advisor.Name}: sin contpaqi_database");
                    continue;
                }

                await _contpaqiSqlService.UpdateAgentFromMidaAsync(
                    advisor.ContpaqiDatabase,
                    advisor.ContpaqiAgentId.Value,
                    advisor.Name,
                    advisor.ContpaqiCode,
                    advisor.Active
                );

                SyncMetadataService.MarkAsSyncedFromContpaqi(advisor);
                await advisor.Update<Advisor>();

                result.Updated++;
            }
            catch (Exception ex)
            {
                SyncMetadataService.MarkAsError(advisor, ex.Message);
                await advisor.Update<Advisor>();

                result.Errors.Add($"{advisor.Name}: {ex.Message}");
            }
        }

        return result;
    }
}