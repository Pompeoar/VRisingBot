using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VRisingBot.Services;


IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var serviceProvider = CreateProvider(config);

await MainAsync();

async Task MainAsync()
{
    await InitializeServices(serviceProvider);

    var socketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();
    await socketClient.LoginAsync(TokenType.Bot, config["DiscordBotToken"]);
    await socketClient.StartAsync();
    // Block this task until the program is closed.
    await Task.Delay(Timeout.Infinite);
}

static IServiceProvider CreateProvider(IConfiguration config) => 
    new ServiceCollection()
        .AddSingleton(new DiscordSocketConfig())
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton<CommandService>()
        .AddSingleton<CommandHandler>()
        .AddSingleton<LoggingService>()
        .AddSingleton<AzureContainerInstanceService>()
        .BuildServiceProvider();

static async Task InitializeServices(IServiceProvider serviceProvider)
{
    serviceProvider.GetRequiredService<IConfiguration>();
    serviceProvider.GetRequiredService<AzureContainerInstanceService>();
    serviceProvider.GetRequiredService<LoggingService>();
    await serviceProvider
        .GetRequiredService<CommandHandler>()
        .InstallCommandsAsync();
}

