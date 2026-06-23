namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiDocumentMovementDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public double NumeroMovimiento { get; set; }
    public int ProductoId { get; set; }
    public double Unidades { get; set; }
    public double Precio { get; set; }
    public double Neto { get; set; }
    public double Impuesto1 { get; set; }
    public double Total { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}