using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using SoporteMida.Api.Services.Sync;

namespace SoporteMida.Api.Models;

[Table("advisors")]
public class Advisor : BaseModel, ISyncEntity
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

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("last_local_change_at")]
    public DateTime? LastLocalChangeAt { get; set; }

    [Column("last_remote_change_at")]
    public DateTime? LastRemoteChangeAt { get; set; }

    [Column("sync_source")]
    public string? SyncSource { get; set; }

    [Column("sync_status")]
    public string? SyncStatus { get; set; }

    [Column("sync_error")]
    public string? SyncError { get; set; }
}