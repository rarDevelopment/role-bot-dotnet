global using Discord;
global using Discord.Interactions;
global using Discord.WebSocket;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
using DiscordDotNetUtilities;
using DiscordDotNetUtilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoleBot;
using RoleBot.Models;
using Serilog;
using System.Reflection;
using RoleBot.BusinessLayer;
using RoleBot.DataLayer;
using RoleBot.Helpers;
using RoleBot.EventHandlers;
using RoleBot.Notifications;

var builder = new HostBuilder();

builder.ConfigureAppConfiguration(options
    => options.AddJsonFile("appsettings.json")
        .AddUserSecrets(Assembly.GetEntryAssembly()!, true)
        .AddEnvironmentVariables())
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.AddEnvironmentVariables(prefix: "DOTNET_");
    });

var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"logs/log-{DateTime.Now:dd.MM.yy_HH.mm}.log")
    .CreateLogger();

builder.ConfigureServices((host, services) =>
{
    services.AddLogging(options => options.AddSerilog(loggerConfig, dispose: true));
    services.AddSingleton(new DiscordSocketClient(
        new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All,
            FormatUsersInBidirectionalUnicode = false,
            AlwaysDownloadUsers = true,
            LogGatewayIntentWarnings = false
        }));

    var discordSettings = new DiscordSettings
    {
        BotToken = host.Configuration["Discord:BotToken"]
    };

    var databaseSettings = new DatabaseSettings
    {
        Cluster = host.Configuration["Database:Cluster"],
        User = host.Configuration["Database:User"],
        Password = host.Configuration["Database:Password"],
        Name = host.Configuration["Database:Name"],
    };

    var versionSettings = new VersionSettings
    {
        VersionNumber = host.Configuration["Version:VersionNumber"]
    };

    services.AddSingleton(discordSettings);
    services.AddSingleton(databaseSettings);
    services.AddSingleton(versionSettings);

    services.AddScoped<RoleHelper>();
    services.AddScoped<IRoleDataLayer, RoleDataLayer>();
    services.AddScoped<IRoleBusinessLayer, RoleBusinessLayer>();
    services.AddScoped<IColorRoleDataLayer, ColorRoleDataLayer>();
    services.AddScoped<IColorRoleBusinessLayer, ColorRoleBusinessLayer>();
    services.AddScoped<IConfigurationBusinessLayer, ConfigurationBusinessLayer>();
    services.AddScoped<IConfigurationDataLayer, ConfigurationDataLayer>();
    services.AddScoped<IDiscordFormatter, DiscordFormatter>();

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

    services.AddSingleton<InteractionHandler>();

    // Register EventBus
    services.AddScoped<IEventBus, EventBus>();

    // Register Event Handlers
    services.AddScoped<IEventHandler<MessageReceivedNotification>, MessageReceivedNotificationHandler>();
    services.AddScoped<IEventHandler<UserJoinedNotification>, UserJoinedNotificationHandler>();
    services.AddScoped<IEventHandler<JoinedGuildNotification>, JoinedGuildNotificationHandler>();

    services.AddHostedService<DiscordBot>();
});

var app = builder.Build();

await app.RunAsync();
