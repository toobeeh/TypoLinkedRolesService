using tobeh.TypoLinkedRolesService.Server.Util;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class PalantirMetadataRegistrationService(
    DiscordAppMetadataService discordAppMetadataService, 
    ILogger<PalantirMetadataRegistrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("StartAsync()");
        
        var scheme = await discordAppMetadataService.SetMetadataDefinition(PalantirMetadata.Definitions);
        logger.LogInformation("Updated the metadata scheme: {scheme}", scheme);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}