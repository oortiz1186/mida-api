namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiAdditionalContactDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public bool Active { get; set; } = true;
}