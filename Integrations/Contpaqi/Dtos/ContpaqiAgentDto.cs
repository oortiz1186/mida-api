namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiAgentDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public DateTime FechaAlta { get; set; }
}