using System.Collections.Concurrent;
using tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class MetadataEligibilityService(ILogger<MetadataEligibilityService> logger)
{
    private readonly ConcurrentDictionary<long, MetadataRecordDto> _metadataRecords = new ();

    public bool MetadataIsEligibleForUpdate(long userId, PalantirMetadataDto metadata)
    {
        logger.LogTrace("MetadataIsEligibleForUpdate(userId={userId}, metadata={metadata})", userId, metadata);
        
        // if no record exists, it is eligible
        if(!_metadataRecords.TryGetValue(userId, out var record))
        {
            return true;
        }

        var oldMetadata = record.Metadata;
        
        // if last update was more than an hour ago, it is eligible
        if(DateTimeOffset.UtcNow - record.CreatedAt > TimeSpan.FromHours(1))
        {
            return true;
        }
        
        // if patron or patronizer status has changed, it is eligible
        if(oldMetadata.Patron != metadata.Patron || oldMetadata.Patronizer != metadata.Patronizer) return true;
        
        // if more than 100 bubbles changed, it is eligible
        if(Math.Abs(oldMetadata.Bubbles - metadata.Bubbles) > 100) return true;
        
        // if more than 100 drops changed, it is eligible
        if(Math.Abs(oldMetadata.Drops - metadata.Drops) > 100) return true;

        return false;
    }
    
    public void LogMetadataRecord(long userId, PalantirMetadataDto metadata)
    {
        logger.LogTrace("LogMetadataRecord(userId={userId}, metadata={metadata})", userId, metadata);
        
        _metadataRecords.AddOrUpdate(userId, new MetadataRecordDto(DateTimeOffset.UtcNow, metadata), (_, _) => new MetadataRecordDto(DateTimeOffset.UtcNow, metadata));
    }
}