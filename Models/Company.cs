using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SoporteMida.Api.Models;

[Table("ticket_companies")]
public class Company : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("rfc")]
    public string? Rfc { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }
}