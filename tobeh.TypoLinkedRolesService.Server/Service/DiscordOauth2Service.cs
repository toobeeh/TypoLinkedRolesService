using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.Database;
using tobeh.TypoLinkedRolesService.Server.Database.Model;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class DiscordOauth2Service
{
    private readonly HttpClient _httpClient;
    private readonly DiscordOauthConfig _config;
    private readonly ILogger<DiscordOauth2Service> _logger;
    private readonly AppDatabaseContext _db;
    private static readonly string SecretStateKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public DiscordOauth2Service(IHttpClientFactory httpClientFactory, ILogger<DiscordOauth2Service> logger, IOptions<DiscordOauthConfig> config, AppDatabaseContext dbContext)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger; 
        _config = config.Value;
        _db = dbContext;
        
        _httpClient.BaseAddress = new Uri("https://discord.com/api/");
    }

    public async Task<DiscordOauth2TokenDto> GetOauthToken(string code)
    {
        _logger.LogTrace("GetOauthToken(code={code}", code);
       
        // Create the body for the request
        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _config.ClientId),
            new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", _config.RedirectUrl),
        });

        var response = await _httpClient.PostAsync("oauth2/token", body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DiscordOauth2TokenDto>() ?? throw new NullReferenceException("token is null");
    }

    public async Task<DiscordOauth2TokenDto> RefreshOauthToken(string refreshToken)
    {
        _logger.LogTrace("RefreshOauthToken(refreshToken={refreshToken})", refreshToken);
        
        // Create the body for the request
        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _config.ClientId),
            new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
        });

        var response = await _httpClient.PostAsync("oauth2/token", body);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DiscordOauth2TokenDto>() ?? throw new NullReferenceException("token is null");
    }

    public async Task<ulong> GetDiscordUserId(string accessToken)
    {
        _logger.LogTrace("GetDiscordUserId(accessToken={accessToken}", accessToken);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync($"oauth2/@me");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DiscordOauthUserDto>();
        return result?.User.Id ?? throw new NullReferenceException("No metadata returned");
    }

    public async Task SaveUserToken(ulong userId, DiscordOauth2TokenDto token)
    {
        _logger.LogTrace("SaveUserToken(userId={userId}, tokens={tokens}", userId, token);

        var existing = await _db.DiscordUserTokens.FirstOrDefaultAsync(token => token.UserId == userId);
        if (existing is not null)
        {
            _db.DiscordUserTokens.Remove(existing);
            await _db.SaveChangesAsync();
        }

        _db.DiscordUserTokens.Add(new DiscordUserToken
        {
            AccessToken = token.AccessToken, RefreshToken = token.RefreshToken, UserId = userId,
            Expiry = DateTime.UtcNow.AddDays(7)
        });
        await _db.SaveChangesAsync();
    }

    public async Task<DiscordOauth2TokenDto> GetSavedUserToken(ulong userId)
    {
        _logger.LogTrace("GetSavedUserToken(userId={userId}", userId);
        
        var token = await _db.DiscordUserTokens.FirstOrDefaultAsync(token => token.UserId == userId);
        if(token is null) throw new NullReferenceException("No token saved");

        if (token.Expiry > DateTime.UtcNow)
            return new DiscordOauth2TokenDto(token.AccessToken, token.RefreshToken);
        
        var refreshedToken = await RefreshOauthToken(token.RefreshToken);
        await SaveUserToken(userId, refreshedToken);
        return refreshedToken;

    }

    public string GetAuthorizationUrl()
    {
        return
            $"https://discord.com/oauth2/authorize?client_id={_config.ClientId}&response_type=code&redirect_uri={HttpUtility.UrlEncode(_config.RedirectUrl)}&connect&scope=identify+role_connections.write";
    }
    
    public string GetStateSecret()
    {
        return SecretStateKey;
    }
    
}