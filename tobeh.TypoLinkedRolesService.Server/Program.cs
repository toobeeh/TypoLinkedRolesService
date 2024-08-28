using System.Globalization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using tobeh.TypoLinkedRolesService.Server.Config;
using tobeh.TypoLinkedRolesService.Server.DiscordDtos;
using tobeh.TypoLinkedRolesService.Server.Grpc;
using tobeh.TypoLinkedRolesService.Server.Service;
using tobeh.Valmar.Client.Util;

namespace tobeh.TypoLinkedRolesService.Server;

class Program
{
    public static async Task Main(string[] args)
    {
        SetupCulture();
        var builder = SetupApp(args);
        var app = builder.Build();
        SetupRoutes(app);

        /*var s = app.Services.CreateScope();
        var test = s.ServiceProvider.GetService<DiscordLinkedRolesService>();
        var res = await test.SetMetadataDefinition([
            new MetadataDefinitionDto(MetadataDefinitionTypeDto.IntegerEqual, "bubbles", "Bubbles",
                "Amount of bubbles collected")
        ]);*/
       
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
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddHttpClient();
        builder.Services.AddLogging();
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValmarGrpc(
            builder.Configuration.GetRequiredSection("Grpc").GetValue<string>("ValmarAddress") ??
            throw new ArgumentException("No Valmar URL provided"));
        
        builder.Services.AddScoped<DiscordLinkedRolesService>();
        builder.Services.Configure<DiscordClientConfig>(builder.Configuration.GetSection("DiscordClient"));
        
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