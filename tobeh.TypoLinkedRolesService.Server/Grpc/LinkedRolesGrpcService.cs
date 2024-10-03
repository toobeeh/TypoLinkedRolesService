using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service;

namespace tobeh.TypoLinkedRolesService.Server.Grpc;

public class LinkedRolesGrpcService(
    PalantirMetadataService palantirMetadataService, 
    DiscordAppMetadataService discordAppMetadataService, 
    DiscordOauth2Service discordOauth2Service, 
    MetadataEligibilityService metadataEligibilityService,
    ILogger<LinkedRolesGrpcService> logger) : LinkedRoles.LinkedRolesBase
{
    public override async Task<Empty> UpdateUserMetadata(UpdateUserMetadataMessage request, ServerCallContext context)
    {
        logger.LogTrace("UpdateUserMetadata(request={request})", request);

        var tokensDict = new Dictionary<long, DiscordOauth2TokenDto>();
        foreach (var id in request.UserIds)
        {
            if(tokensDict.ContainsKey(id)) continue;
            
            DiscordOauth2TokenDto token;
            try
            {
                token = await discordOauth2Service.GetSavedUserToken((ulong)id);
            }
            catch(Exception ex)
            {
                logger.LogDebug("Failed to get user token for {id}: {ex}", id, ex);
                continue;
            }
           
            tokensDict.Add(id, token);
        }
        
        logger.LogInformation("Found {n} ids to update", tokensDict.Count);
        
        var tasks = tokensDict.Keys.Select(id => Task.Run(async () =>
        {
            try
            {
                var tokens = tokensDict[id];
                var metadata = await palantirMetadataService.GetMetadataForMember((ulong)id);

                if (metadataEligibilityService.MetadataIsEligibleForUpdate(id, metadata.PalantirMetadata))
                {
                    try
                    {
                        await discordAppMetadataService.PushUserMetadata(metadata, tokens.AccessToken);
                    }
                    catch (RateLimitedException e)
                    {
                        logger.LogWarning("Rate limited, retry in {e.RetryIn} seconds", e.RetryIn);
                        return;
                    }
                    metadataEligibilityService.LogMetadataRecord(id, metadata.PalantirMetadata);
                }
                else
                {
                    logger.LogDebug("Metadata for user {id} is not eligible for update", id);
                }
            }
            catch (Exception e)
            {
                logger.LogError("Failed to update metadata for user {id}: {e}", id, e);
            }
        })).ToArray();

        await Task.WhenAll(tasks);
        logger.LogInformation("Finished update");
        
        return new Empty();
    }
}