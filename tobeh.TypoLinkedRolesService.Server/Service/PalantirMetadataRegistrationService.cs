using tobeh.TypoLinkedRolesService.Server.Util;

namespace tobeh.TypoLinkedRolesService.Server.Service;

public class PalantirMetadataRegistrationService(
    IServiceScopeFactory serviceScopeFactory, 
    ILogger<PalantirMetadataRegistrationService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("StartAsync()");

        using var scope = serviceScopeFactory.CreateScope();
        var discordAppMetadataService = scope.ServiceProvider.GetRequiredService<DiscordAppMetadataService>();
        
        // get current scheme
        var currentScheme = (await discordAppMetadataService.GetMetadataDefinition()).ToList();
        var newScheme = PalantirMetadata.Definitions.ToList();
        
        // check if it differs
        var schemeIsOutdated = newScheme.Any(newItem => !currentScheme.Any(oldItem => 
            oldItem.Name == newItem.Name && newItem.Description == oldItem.Description && 
            newItem.Description == oldItem.Description && newItem.Key == oldItem.Key ));

        if (schemeIsOutdated)
        {
            logger.LogInformation("Current metadata scheme is different to definition, updating");
            currentScheme = (await discordAppMetadataService.SetMetadataDefinition(PalantirMetadata.Definitions)).ToList();
        }
        
        logger.LogInformation("Current metadata scheme: {scheme}", currentScheme);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}