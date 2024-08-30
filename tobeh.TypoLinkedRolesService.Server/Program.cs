using System.Globalization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.Database;
using tobeh.TypoLinkedRolesService.Server.Grpc;
using tobeh.TypoLinkedRolesService.Server.Service;
using tobeh.Valmar.Client.Util;

namespace tobeh.TypoLinkedRolesService.Server;

class Program
{
    public static async Task Main(string[] args)
    {
        SetupCulture();
        AppDatabaseContext.EnsureDatabaseExists();
        var builder = SetupApp(args);
        var app = builder.Build();
        SetupRoutes(app);
        
        await app.RunAsync();
    }

    private static void SetupCulture()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }

    private static WebApplicationBuilder SetupApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // configure kestrel
        builder.WebHost.ConfigureKestrel(options =>
        {
            // setup linked roles api endpoint with http/1
            options.ListenAnyIP(builder.Configuration.GetRequiredSection("Rest").GetValue<int>("HostPort"), o => o.Protocols = HttpProtocols.Http1);
            
            // Setup a HTTP/2 endpoint without TLS for grpc.
            options.ListenAnyIP(builder.Configuration.GetRequiredSection("Grpc").GetValue<int>("HostPort"), o => o.Protocols = HttpProtocols.Http2);
        });

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddControllers();
        
        builder.Services
            .AddRouting(options => options.LowercaseUrls = true)
            .AddLogging()
            .AddSwaggerGen()
            .AddEndpointsApiExplorer()
            .AddHttpClient()
            .AddDbContext<AppDatabaseContext>()
            .AddHostedService<PalantirMetadataRegistrationService>()
            .AddScoped<DiscordAppMetadataService>()
            .AddScoped<DiscordOauth2Service>()
            .AddScoped<PalantirMetadataService>()
            .Configure<DiscordClientConfig>(builder.Configuration.GetSection("DiscordClient"))
            .Configure<DiscordOauthConfig>(builder.Configuration.GetSection("DiscordOauth"))
            .AddValmarGrpc(
                builder.Configuration.GetRequiredSection("Grpc").GetValue<string>("ValmarAddress") ??
                throw new ArgumentException("No Valmar URL provided"));
        
        return builder;
    }

    private static void SetupRoutes(WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        // Configure the HTTP request pipeline
        app.UseRouting();
        app.MapGrpcService<LinkedRolesGrpcService>();
        app.MapControllers();
    }
}