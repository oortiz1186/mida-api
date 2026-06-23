namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiDocumentDto
{
    public int Id { get; set; }
    public int ConceptoId { get; set; }
    public string Serie { get; set; } = string.Empty;
    public double Folio { get; set; }
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string Rfc { get; set; } = string.Empty;
    public double Neto { get; set; }
    public double Impuesto1 { get; set; }
    public double Total { get; set; }
    public double Pendiente { get; set; }
    public int Cancelado { get; set; }
    public string Referencia { get; set; } = string.Empty;
}