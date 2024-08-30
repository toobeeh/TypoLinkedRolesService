using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service;

namespace tobeh.TypoLinkedRolesService.Server.Grpc;

public class LinkedRolesGrpcService(
    PalantirMetadataService palantirMetadataService, 
    DiscordAppMetadataService discordAppMetadataService, 
    DiscordOauth2Service discordOauth2Service, 
    ILogger<LinkedRolesGrpcService> logger) : LinkedRoles.LinkedRolesBase
{
    public override async Task<Empty> UpdateUserMetadata(UpdateUserMetadataMessage request, ServerCallContext context)
    {
        logger.LogTrace("UpdateUserMetadata(request={request})", request);

        var tokensDict = new Dictionary<long, DiscordOauth2TokenDto>();
        foreach (var id in request.UserIds)
        {
            DiscordOauth2TokenDto token;
            try
            {
                token = await discordOauth2Service.GetSavedUserToken((ulong)id);
            }
            catch(Exception ex)
            {
                logger.LogWarning("Failed to get user token for {id}: {ex}", id, ex);
                continue;
            }
           
            tokensDict.Add(id, token);
        }
        
        logger.LogInformation("Found {n} ids to update", tokensDict.Count);
        
        var tasks = tokensDict.Keys.Select(id => Task.Run(async () =>
        {
            var tokens = tokensDict[id];
            var metadata = await palantirMetadataService.GetMetadataForMember((ulong)id);
            await discordAppMetadataService.PushUserMetadata(metadata, tokens.AccessToken);
        })).ToArray();

        await Task.WhenAll(tasks);
        logger.LogInformation("Finished update");
        
        return new Empty();
    }
}