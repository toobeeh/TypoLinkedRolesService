using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service;

namespace tobeh.TypoLinkedRolesService.Server.Grpc;

public class LinkedRolesGrpcService(
    PalantirMetatadaService palantirMetatadaService, 
    DiscordAppMetadataService discordAppMetadataService, 
    DiscordOauth2Service discordOauth2Service, 
    ILogger<LinkedRolesGrpcService> logger) : LinkedRoles.LinkedRolesBase
{
    public override async Task<Empty> UpdateUserMetadata(UpdateUserMetadataMessage request, ServerCallContext context)
    {
        logger.LogTrace("UpdateUserMetadata(request={request})", request);

        var tokensDict = new Dictionary<long, DiscordOauth2TokensDto>();
        foreach (var id in request.UserIds)
        {
            var tokens = await discordOauth2Service.GetSavedUserTokens((ulong)id); 
            tokensDict.Add(id, tokens);
        }
        
        var tasks = tokensDict.Keys.Select(id => Task.Run(async () =>
        {
            var tokens = tokensDict[id];
            var metadata = await palantirMetatadaService.GetMetadataForMember((ulong)id);
            await discordAppMetadataService.PushUserMetadata(metadata, tokens.AccessToken);
        })).ToArray();

        await Task.WhenAll(tasks);
        
        return new Empty();
    }
}