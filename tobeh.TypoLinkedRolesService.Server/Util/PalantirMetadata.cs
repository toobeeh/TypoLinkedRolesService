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
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "ispatron", "Patron", "User is a typo patreon subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "ispatronizer", "Patronizer", "User is a typo patronizer tier subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "ismember", "Member", "User has a typo account")
    };
}