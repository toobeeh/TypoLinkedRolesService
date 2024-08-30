using tobeh.TypoLinkedRolesService.Server.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Util;

public static class PalantirMetadata
{
    public static readonly MetadataDefinitionDto[] Definitions =
    {
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.IntegerGreaterThanOrEqual, "bubbles", "Bubbles",
            "bubbles collected"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.IntegerGreaterThanOrEqual, "drops", "Drops",
            "drops collected"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "patron", "Patron", "User is a typo patreon subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "patronizer", "Patronizer", "User is a typo patronizer subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "member", "Member", "User has a typo account")
    };
}