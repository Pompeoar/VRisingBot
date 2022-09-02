
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace VRisingBot.Services
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly IServiceProvider services;
        private readonly InteractionService interactionCommands;
        private readonly ulong testGuildId;

        public SlashCommandHandler(
            DiscordSocketClient client,
            IServiceProvider services,
            InteractionService interactionCommands)
        {
            this.services = services;
            this.interactionCommands = interactionCommands;
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

            // process the command execution results 
            interactionCommands.SlashCommandExecuted += SlashCommandExecuted;
            interactionCommands.ContextCommandExecuted += ContextCommandExecuted;
            interactionCommands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private async Task ReadyAsync()
        {
            if (IsDebug())
            {
                Console.WriteLine($"In debug mode, adding commands to {testGuildId}...");
                await interactionCommands.RegisterCommandsToGuildAsync(testGuildId);
            }
            else
            {
                // Reminder: This can take an hour
                await interactionCommands.RegisterCommandsGloballyAsync(true);
            }
            Console.WriteLine($"Connected as -> [{client.CurrentUser}] :)");
        }

        static bool IsDebug()
        {
            #if DEBUG
                        return true;
            #endif
        }

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
        
        private Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }
        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
