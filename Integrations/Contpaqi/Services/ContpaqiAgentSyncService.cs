using SoporteMida.Api.Integrations.Contpaqi.Dtos;
using SoporteMida.Api.Integrations.Contpaqi.Services;
using SoporteMida.Api.Models;

namespace SoporteMida.Api.Services;

public class ContpaqiAgentSyncService
{
    private readonly ContpaqiSqlService _contpaqiSqlService;
    private readonly SupabaseClientService _supabase;

    public ContpaqiAgentSyncService(
        ContpaqiSqlService contpaqiSqlService,
        SupabaseClientService supabase)
    {
        _contpaqiSqlService = contpaqiSqlService;
        _supabase = supabase;
    }

    public async Task<SyncResultDto> SyncAgentsAsync(string databaseName)
    {
        var result = new SyncResultDto();

        var agents = await _contpaqiSqlService.GetAgentsAsync(databaseName);
        result.TotalRead = agents.Count;

        foreach (var agent in agents)
        {
            try
            {
                var existing = await _supabase.Client
                    .From<Advisor>()
                    .Where(x => x.ContpaqiDatabase == databaseName)
                    .Where(x => x.ContpaqiAgentId == agent.Id)
                    .Get();

                var advisor = existing.Models.FirstOrDefault();

                if (advisor is null)
                {
                    advisor = new Advisor
                    {
                        Id = Guid.NewGuid(),
                        Name = agent.Nombre,
                        Role = "soporte",
                        Active = true,
                        ContpaqiAgentId = agent.Id,
                        ContpaqiCode = agent.Codigo,
                        ContpaqiDatabase = databaseName,
                        LastSyncedAt = DateTime.UtcNow
                    };

                    await _supabase.Client
                        .From<Advisor>()
                        .Insert(advisor);

                    result.Created++;
                }
                else
                {
                    advisor.Name = agent.Nombre;
                    advisor.Role = "Agente CONTPAQi";
                    advisor.Active = true;
                    advisor.ContpaqiCode = agent.Codigo;
                    advisor.LastSyncedAt = DateTime.UtcNow;

                    await advisor.Update<Advisor>();

                    result.Updated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"{agent.Codigo} - {agent.Nombre}: {ex.Message}");
            }
        }

        return result;
    }
}