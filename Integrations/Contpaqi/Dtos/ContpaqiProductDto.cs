namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiProductDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? TipoProducto { get; set; }
    public int Estatus { get; set; }
}