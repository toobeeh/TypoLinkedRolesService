using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
        
        var tasks = request.UserIds.Select(id => Task.Run(async () =>
        {
            var tokens = await discordOauth2Service.GetSavedUserTokens((ulong)id);
            var metadata = await palantirMetatadaService.GetMetadataForMember((ulong)id);
            await discordAppMetadataService.PushUserMetadata(metadata, tokens.AccessToken);
        })).ToArray();

        await Task.WhenAll(tasks);
        
        return new Empty();
    }
}