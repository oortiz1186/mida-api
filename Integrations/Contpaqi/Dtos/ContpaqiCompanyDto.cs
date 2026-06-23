namespace SoporteMida.Api.Integrations.Contpaqi.Dtos;

public class ContpaqiCompanyDto
{
    public long IdDatabase { get; set; }
    public Guid GuidCompany { get; set; }
    public string? Version { get; set; }
    public string? NombreEmpresa { get; set; }
    public string? Alias { get; set; }
    public string? Rfc { get; set; }
    public string? CompanyPath { get; set; }
    public string? DatabaseDocumentsMetadata { get; set; }
    public string? DatabaseDocumentsContent { get; set; }
}