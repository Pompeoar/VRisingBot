
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace VRisingBot.Services
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider services;
        private readonly InteractionService interactionCommands;
        private readonly AzureContainerInstanceService aciService;
        private readonly ulong testGuildId;
        private const string VRISING_OPTION = "v-rising-server-options";

        public SlashCommandHandler(
            DiscordSocketClient client,
            IServiceProvider services,
            InteractionService interactionCommands,
            AzureContainerInstanceService aciService)
        {
            this.services = services;
            this.interactionCommands = interactionCommands;
            this.aciService = aciService;
            this.client = client;
            ulong.TryParse(this.services.GetRequiredService<IConfiguration>()["TestGuildId"], out testGuildId);
        }

        public async Task InstallCommandsAsync()
        {
            client.Ready += ReadyAsync;
            await interactionCommands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                                        services: services);

            // process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += HandleInteraction;
            client.SlashCommandExecuted += CommandHandler;
        }

        private async Task ReadyAsync()
        {
            var guildCommand = new SlashCommandBuilder().WithGameServerOptions(VRISING_OPTION);

            if (IsDebug())
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), testGuildId);
            }
            else
            {
                // Reminder: This can take an hour
                await client.CreateGlobalApplicationCommandAsync(guildCommand.Build());
            }
            Console.WriteLine($"Connected as -> [{client.CurrentUser}] :)");
        }

        static bool IsDebug() =>
            Debugger.IsAttached
            ? true
            : false;

        private async Task CommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case VRISING_OPTION:
                    await HandleServerCommand(command);
                    break;         
            }
        }

        private async Task HandleServerCommand(SocketSlashCommand command) =>
            await command.RespondAsync(await aciService.ExecuteCommand(Enum.Parse<ContainerCommand>(command.Data.Options.First().Value.ToString())));
       

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(client, arg);
                await interactionCommands.ExecuteCommandAsync(ctx, services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
    }

    public static class SlashCommandBuilderExtensions
    {
        public static SlashCommandBuilder WithGameServerOptions(this SlashCommandBuilder builder, string serverName) =>
            builder.WithName(serverName)
             .WithDescription("A command to interact with the game server..")
             .AddOption(new SlashCommandOptionBuilder()
                 .WithName("command")
                 .WithDescription("Choose what you want to happen.")
                 .WithRequired(true)
                 .AddChoice(Enum.GetName(ContainerCommand.Start), ContainerCommand.Start.ToString())
                 .AddChoice(Enum.GetName(ContainerCommand.Stop), ContainerCommand.Stop.ToString())
                 .AddChoice(Enum.GetName(ContainerCommand.Restart), ContainerCommand.Restart.ToString())
                 .AddChoice(Enum.GetName(ContainerCommand.GetCurrentState), ContainerCommand.GetCurrentState.ToString())
                 .WithType(ApplicationCommandOptionType.String));
    }
}
