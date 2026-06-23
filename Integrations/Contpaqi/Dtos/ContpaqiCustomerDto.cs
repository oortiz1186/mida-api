namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiCustomerDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public string? Email { get; set; }
    public string? UsoCfdi { get; set; }
    public string? RegimenFiscal { get; set; }
    public string? Whatsapp { get; set; }
    public int Estatus { get; set; }
    public string? Email2 { get; set; }
    public string? Email3 { get; set; }
}