using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class DiscordLinkedRolesService
{
    private readonly HttpClient _httpClient;
    private readonly DiscordClientConfig _config;
    private readonly ILogger<DiscordLinkedRolesService> _logger;

    public DiscordLinkedRolesService(IHttpClientFactory httpClientFactory, ILogger<DiscordLinkedRolesService> logger, IOptions<DiscordClientConfig> config)
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
        _logger.LogInformation("SetMetadataDefinition(metadata: {metadata})", metadata);
        
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
        _logger.LogInformation("GetMetadataDefinition()");
        
        var response = await _httpClient.GetAsync($"applications/{_config.ApplicationId}/role-connections/metadata");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<MetadataDefinitionDto[]>();
        return result ?? throw new NullReferenceException("No metadata returned");
    }
    
}