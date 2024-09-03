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
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "is_patron", "Patron", "User is a typo patreon subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "is_patronizer", "Patronizer", "User is a typo patronizer subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, "is_member", "Member", "User has a typo account")
    };
}