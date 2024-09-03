using tobeh.TypoLinkedRolesService.Server.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Util;

public static class PalantirMetadata
{
    public const string BubblesMetadataKey = "bubbles";
    public const string DropsMetadataKey = "drops";
    public const string PatronMetadataKey = "is_patron";
    public const string PatronizerMetadataKey = "is_patronizer";
    public const string MemberMetadataKey = "is_member";
    
    public static readonly MetadataDefinitionDto[] Definitions =
    {
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.IntegerGreaterThanOrEqual, BubblesMetadataKey, "Bubbles",
            "bubbles collected"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.IntegerGreaterThanOrEqual, DropsMetadataKey, "Drops",
            "drops collected"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, PatronMetadataKey, "Patron", "User is a typo patreon subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, PatronizerMetadataKey, "Patronizer", "User is a typo patronizer tier subscriber"),
        new MetadataDefinitionDto(MetadataDefinitionTypeDto.BooleanEqual, MemberMetadataKey, "Member", "User has a typo account")
    };
}