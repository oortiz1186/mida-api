using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SoporteMida.Api.Models;

[Table("contact_companies")]
public class ContactCompany : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("contact_id")]
    public Guid ContactId { get; set; }

    [Column("company_id")]
    public Guid CompanyId { get; set; }

    [Column("is_primary")]
    public bool IsPrimary { get; set; } = true;

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}