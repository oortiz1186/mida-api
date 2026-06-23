using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SoporteMida.Api.Models;

[Table("advisors")]
public class Advisor : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("role")]
    public string? Role { get; set; }

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("contpaqi_agent_id")]
    public int? ContpaqiAgentId { get; set; }

    [Column("contpaqi_code")]
    public string? ContpaqiCode { get; set; }

    [Column("contpaqi_database")]
    public string? ContpaqiDatabase { get; set; }

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }
}