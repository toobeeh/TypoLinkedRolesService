using System.Text.Json.Serialization;

namespace tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

public record DiscordOauthUserDto(
    [property: JsonPropertyName("user")] DiscordUserDto User);
    
public record DiscordUserDto(
    [property: JsonPropertyName("id")] ulong Id);