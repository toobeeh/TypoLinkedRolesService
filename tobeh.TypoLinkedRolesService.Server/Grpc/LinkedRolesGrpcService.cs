using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace tobeh.TypoLinkedRolesService.Server.Grpc;

public class LinkedRolesGrpcService : LinkedRoles.LinkedRolesBase
{
    public override async Task<HiResponse> SayHi(Empty request, ServerCallContext context)
    {
        return new HiResponse {Message = "Hi!"};
    }
}