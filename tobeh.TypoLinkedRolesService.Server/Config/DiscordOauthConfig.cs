namespace tobeh.TypoLinkedRolesService.Server.Config;

public class DiscordOauthConfig
{
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string RedirectUrl { get; init; }
}