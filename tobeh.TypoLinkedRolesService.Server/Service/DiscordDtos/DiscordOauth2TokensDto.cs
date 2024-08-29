using System.Text.Json.Serialization;

namespace tobeh.TypoLinkedRolesService.Server.DiscordDtos;

public record DiscordOauth2TokensDto(
    [property: JsonPropertyName("access_token")] string AccessToken, 
    [property: JsonPropertyName("refresh_token")] string RefreshToken);