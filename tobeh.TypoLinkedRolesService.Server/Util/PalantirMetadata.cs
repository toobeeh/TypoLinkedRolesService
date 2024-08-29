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
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "patron", "Patron", "is patreon subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "patronizer", "Patronizer", "is patronizer subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "member", "Member", "has a typo account")
    };
}