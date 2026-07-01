namespace SoporteMida.Api.Services.Sync;

public static class SyncConflictResolver
{
    public static bool ShouldSkipContpaqiUpdate<T>(T entity)
        where T : ISyncEntity
    {
        if (entity.SyncSource != SyncSource.Mida ||
            entity.SyncStatus != SyncStatus.Pending)
        {
            return false;
        }

        var localChange = entity.LastLocalChangeAt
            ?? entity.UpdatedAt
            ?? DateTime.MinValue;

        var remoteChange = entity.LastRemoteChangeAt
            ?? entity.LastSyncedAt
            ?? DateTime.MinValue;

        return localChange >= remoteChange;
    }
}