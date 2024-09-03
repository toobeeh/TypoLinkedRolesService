using System.Text.Json.Serialization;
using tobeh.TypoLinkedRolesService.Server.Util;

namespace tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

public record PalantirConnectionDto(
    [property: JsonPropertyName("platform_username")]
    string PlatformUsername,
    [property: JsonPropertyName("platform_name")]
    string PlatformName,
    [property: JsonPropertyName("metadata")]
    PalantirMetadataDto PalantirMetadata);
public record PalantirMetadataDto(
    [property: JsonPropertyName(PalantirMetadata.PatronMetadataKey)] int Patron,
    [property: JsonPropertyName(PalantirMetadata.MemberMetadataKey)] int Member,
    [property: JsonPropertyName(PalantirMetadata.PatronizerMetadataKey)] int Patronizer,
    [property: JsonPropertyName(PalantirMetadata.BubblesMetadataKey)] int Bubbles,
    [property: JsonPropertyName(PalantirMetadata.DropsMetadataKey)] int Drops);

public record MetadataRecordDto(DateTimeOffset CreatedAt, PalantirMetadataDto Metadata);
