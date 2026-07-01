namespace SoporteMida.Api.Services.Sync;

public interface ISyncEntity
{
    string? SyncSource { get; set; }
    string? SyncStatus { get; set; }
    string? SyncError { get; set; }

    DateTime? LastSyncedAt { get; set; }
    DateTime? LastLocalChangeAt { get; set; }
    DateTime? LastRemoteChangeAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}