using System.Text.Json.Serialization;

namespace tobeh.TypoLinkedRolesService.Server.DiscordDtos;

/// <summary>
/// https://discord.com/developers/docs/resources/application-role-connection-metadata#application-role-connection-metadata-object-application-role-connection-metadata-structure
/// </summary>
/// <param name="Type"></param>
/// <param name="Key"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
public record MetadataDefinitionDto(
    [property: JsonPropertyName("type")] MetadataDefinitionTypeDto Type, 
    [property: JsonPropertyName("key")] string Key, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("description")] string Description);


/// <summary>
/// https://discord.com/developers/docs/resources/application-role-connection-metadata#application-role-connection-metadata-object-application-role-connection-metadata-type
/// </summary>
public enum MetadataDefinitionTypeDto
{
    IntegerLessThanOrEqual = 1,
    IntegerGreaterThanOrEqual = 2,
    IntegerEqual = 3,
    IntegerNotEqual = 4,
    DatetimeLessThanOrEqual = 5,
    DatetimeGreaterThanOrEqual = 6,
    BooleanEqual = 7,
    BooleanNotEqual = 8
}