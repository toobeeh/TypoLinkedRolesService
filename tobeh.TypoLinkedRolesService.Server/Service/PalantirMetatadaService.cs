using tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;
using tobeh.Valmar;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class PalantirMetatadaService(Members.MembersClient membersClient, Inventory.InventoryClient inventoryClient, ILogger<PalantirMetatadaService> logger)
{
    public async Task<PalantirConnectionDto> GetMetadataForMember(ulong discordId)
    {
        logger.LogTrace("GetMetadataForMember(discordId={discordId})", discordId);

        MemberReply member;
        DropCreditReply dropCredit;
        try
        {
            member = await membersClient.GetMemberByDiscordIdAsync(new IdentifyMemberByDiscordIdRequest
                { Id = (long)discordId });
            dropCredit = await inventoryClient.GetDropCreditAsync(new GetDropCreditRequest { Login = member.Login });
        }
        catch
        {
            return new PalantirConnectionDto(
                "Unregistered",
                "skribbl typo",
                new PalantirMetadataDto(
                    0,
                    0,
                    0,
                    0,
                    0
                ));
        }
        
        return new PalantirConnectionDto(
            member.Username,
            (member.PatronEmoji is not null ? $"[{member.PatronEmoji}] " : "") + "skribbl typo",
            new PalantirMetadataDto(
                member.MappedFlags.Contains(MemberFlagMessage.Patron) ? 1 : 0,
                1,
                member.MappedFlags.Contains(MemberFlagMessage.Patronizer) ? 1 : 0,
                member.Bubbles,
                dropCredit.RegularCount
            ));
    }
}