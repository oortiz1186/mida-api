namespace SoporteMida.Api.Services.Sync;

public static class SyncMetadataService
{
    public static void MarkAsPendingFromMida<T>(T entity)
        where T : ISyncEntity
    {
        var now = DateTime.UtcNow;

        entity.SyncSource = SyncSource.Mida;
        entity.SyncStatus = SyncStatus.Pending;
        entity.SyncError = null;
        entity.LastLocalChangeAt = now;
        entity.UpdatedAt = now;
    }

    public static void MarkAsSyncedFromContpaqi<T>(T entity)
        where T : ISyncEntity
    {
        var now = DateTime.UtcNow;

        entity.SyncSource = SyncSource.Contpaqi;
        entity.SyncStatus = SyncStatus.Synced;
        entity.SyncError = null;
        entity.LastRemoteChangeAt = now;
        entity.LastSyncedAt = now;
        entity.UpdatedAt = now;
    }

    public static void MarkAsError<T>(T entity, string error)
        where T : ISyncEntity
    {
        entity.SyncStatus = SyncStatus.Error;
        entity.SyncError = error;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}