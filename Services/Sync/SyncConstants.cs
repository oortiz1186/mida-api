namespace SoporteMida.Api.Services.Sync;

public static class SyncSource
{
    public const string Mida = "mida";
    public const string Contpaqi = "contpaqi";
}

public static class SyncStatus
{
    public const string Pending = "pending";
    public const string Synced = "synced";
    public const string Error = "error";
    public const string Conflict = "conflict";
}