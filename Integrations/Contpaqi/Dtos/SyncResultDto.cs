namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class SyncResultDto
{
    public int TotalRead { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public List<string> Errors { get; set; } = new();
}