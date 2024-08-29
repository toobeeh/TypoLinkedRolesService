using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.Database;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Grpc;
using tobeh.TypoLinkedRolesService.Server.Service;
using tobeh.TypoLinkedRolesService.Server.Util;
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

        var s = app.Services.CreateScope();
        var test = s.ServiceProvider.GetService<DiscordAppMetadataService>();
        var res = await test.SetMetadataDefinition(PalantirMetadata.Definitions);
       
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
        builder.Services.AddDbContext<AppDatabaseContext>();
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddHttpClient();
        builder.Services.AddLogging();
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValmarGrpc(
            builder.Configuration.GetRequiredSection("Grpc").GetValue<string>("ValmarAddress") ??
            throw new ArgumentException("No Valmar URL provided"));
        
        builder.Services.AddScoped<DiscordAppMetadataService>();
        builder.Services.AddScoped<DiscordOauth2Service>();
        builder.Services.AddScoped<PalantirMetatadaService>();
        builder.Services.Configure<DiscordClientConfig>(builder.Configuration.GetSection("DiscordClient"));
        builder.Services.Configure<DiscordOauthConfig>(builder.Configuration.GetSection("DiscordOauth"));
            
        
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
        app.UseHttpsRedirection();
        app.MapGrpcService<LinkedRolesGrpcService>();
        app.MapControllers();
    }
}