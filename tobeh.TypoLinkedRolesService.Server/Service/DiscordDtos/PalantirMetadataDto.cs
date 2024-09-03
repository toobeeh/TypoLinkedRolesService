using System.Text.Json.Serialization;

namespace tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

public record PalantirConnectionDto(
    [property: JsonPropertyName("platform_username")]
    string PlatformUsername,
    [property: JsonPropertyName("platform_name")]
    string PlatformName,
    [property: JsonPropertyName("metadata")]
    PalantirMetadataDto PalantirMetadata);
public record PalantirMetadataDto(
    [property: JsonPropertyName("patron")] int Patron,
    [property: JsonPropertyName("member")] int Member,
    [property: JsonPropertyName("patronizer")] int Patronizer,
    [property: JsonPropertyName("bubbles")] int Bubbles,
    [property: JsonPropertyName("drops")] int Drops);

public record MetadataRecordDto(DateTimeOffset CreatedAt, PalantirMetadataDto Metadata);
