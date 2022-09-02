using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VRisingBot.Services;

IConfiguration config = CreateConfig();

var serviceProvider = CreateServiceProvider(config);

await MainAsync();

async Task MainAsync()
{
    await InitializeServices(serviceProvider);
    await LoginDiscordBot(config, serviceProvider);
    await WaitForever();
}

static IServiceProvider CreateServiceProvider(IConfiguration config) =>
    new ServiceCollection()
        .AddSingleton(new DiscordSocketConfig())
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton<CommandService>()
        .AddSingleton<CommandHandler>()
        .AddSingleton<InteractionService>()
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

static async Task LoginDiscordBot(IConfiguration config, IServiceProvider serviceProvider)
{
    var socketClient = serviceProvider.GetRequiredService<DiscordSocketClient>();
    await socketClient.LoginAsync(TokenType.Bot, config["DiscordBotToken"]);
    await socketClient.StartAsync();
}

static async Task WaitForever() => await Task.Delay(Timeout.Infinite);

static IConfiguration CreateConfig() =>
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.json", optional: true)
        .AddEnvironmentVariables()
        .Build();