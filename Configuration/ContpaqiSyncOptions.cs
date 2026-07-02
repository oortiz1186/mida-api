namespace SoporteMida.Api.Configuration;

public class ContpaqiSyncOptions
{
    public int IntervalMinutes { get; set; } = 5;
    public string DatabaseName { get; set; } = string.Empty;
}