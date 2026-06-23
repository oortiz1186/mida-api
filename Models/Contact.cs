using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SoporteMida.Api.Models;

[Table("contacts")]
public class Contact : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("email")]
    public string? Email { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("contpaqi_customer_id")]
    public int? ContpaqiCustomerId { get; set; }

    [Column("contpaqi_database")]
    public string? ContpaqiDatabase { get; set; }

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}