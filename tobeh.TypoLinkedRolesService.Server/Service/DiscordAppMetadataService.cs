using System.Net;
using System.Net.Http.Headers;
using Grpc.Core;
using Microsoft.Extensions.Options;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Service.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Util;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class DiscordAppMetadataService
{
    private readonly HttpClient _httpClient;
    private readonly DiscordClientConfig _config;
    private readonly ILogger<DiscordAppMetadataService> _logger;

    public DiscordAppMetadataService(IHttpClientFactory httpClientFactory, ILogger<DiscordAppMetadataService> logger, IOptions<DiscordClientConfig> config)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger; 
        _config = config.Value;
        
        _httpClient.BaseAddress = new Uri("https://discord.com/api/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _config.AccessToken);
    }

    /// <summary>
    /// https://discord.com/developers/docs/resources/application-role-connection-metadata#update-application-role-connection-metadata-records
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<MetadataDefinitionDto[]> SetMetadataDefinition(MetadataDefinitionDto[] metadata)
    {
        _logger.LogTrace("SetMetadataDefinition(metadata: {metadata})", metadata);
        
        var response = await _httpClient.PutAsJsonAsync($"applications/{_config.ApplicationId}/role-connections/metadata", metadata);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MetadataDefinitionDto[]>();
        return result ?? throw new NullReferenceException("No metadata returned");
    }
    
    /// <summary>
    /// https://discord.com/developers/docs/resources/application-role-connection-metadata#get-application-role-connection-metadata-records
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<MetadataDefinitionDto[]> GetMetadataDefinition()
    {
        _logger.LogTrace("GetMetadataDefinition()");
        
        var response = await _httpClient.GetAsync($"applications/{_config.ApplicationId}/role-connections/metadata");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MetadataDefinitionDto[]>();
        return result ?? throw new NullReferenceException("No metadata returned");
    }
    
    /// <summary>
    /// https://discord.com/developers/docs/resources/user#update-current-user-application-role-connection
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="accessToken"></param>
    public async Task<PalantirConnectionDto> PushUserMetadata(PalantirConnectionDto metadata, string accessToken)
    {
        _logger.LogTrace("PushUserMetadata(metadata: {metadata})", metadata);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.PutAsJsonAsync($"users/@me/applications/{_config.ApplicationId}/role-connection", metadata);
        
        var headers = response.Headers.Select(kv => $"{kv.Key}: {string.Join(", ", kv.Value)}");
        _logger.LogDebug("Headers for {username}: {headers}", metadata.PlatformUsername, string.Join("; ", headers));
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PalantirConnectionDto>();
        _logger.LogDebug("Pushed metadata: {pushed}, received: {received}", metadata, result);
        return result ?? throw new NullReferenceException("No metadata returned");
    }
    
    /// <summary>
    /// https://discord.com/developers/docs/resources/user#get-current-user-application-role-connection
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<PalantirConnectionDto> GetUserMetadata(string accessToken)
    {
        _logger.LogTrace("PushUserMetadata(accessToken: {accessToken})", accessToken);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync($"users/@me/applications/{_config.ApplicationId}/role-connection");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PalantirConnectionDto>();
        return result ?? throw new NullReferenceException("No metadata returned");
    }
}