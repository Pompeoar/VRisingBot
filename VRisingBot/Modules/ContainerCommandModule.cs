using Discord.Commands;
using VRisingBot.Services;

namespace VRisingBot.Modules
{
    [Group("vrising")]
    public class ContainerModule : ModuleBase<SocketCommandContext>
    {
        private readonly AzureContainerInstanceService aciService;

        public ContainerModule(AzureContainerInstanceService aciService)
        {
            this.aciService = aciService;
        }

        // !vrising help
        [Command("test")]
        [Summary("Starts the VRising Container.")]
        public async Task TestAsync() =>
            await Context.Channel.SendMessageAsync("You not crazy.");

        // !vrising help
        [Command("help")]
        [Summary("Starts the VRising Container.")]
        public async Task HelpAsync() => 
            await Context.Channel.SendMessageAsync("try commands start, stop, restart, or state to interact with the server.");


        // !vrising start
        [Command("start")]
        [Summary("Starts the VRising Container.")]
        public async Task StartAsync()
        {
            await Context.Channel.SendMessageAsync("Attempting to start server ...");
            var result = await aciService.ExecuteCommand(ContainerCommand.Start);
            await Context.Channel.SendMessageAsync(result);
        }

        // !vrising stop
        [Command("stop")]
        [Summary
        ("Stops the VRising Container")]        
        public async Task StopAsync()
        {
            await Context.Channel.SendMessageAsync("Attempting to stop server ...");
            var result = await aciService.ExecuteCommand(ContainerCommand.Stop);
            await Context.Channel.SendMessageAsync(result);
        }

        // !vrising restart
        [Command("restart")]
        [Summary
        ("Restarts a VRising Container")]        
        public async Task RestartAsync()
        {
            await Context.Channel.SendMessageAsync("Attempting to restart server ...");
            var result = await aciService.ExecuteCommand(ContainerCommand.Restart);
            await Context.Channel.SendMessageAsync(result);
        }

        // !vrising restart
        [Command("state")]
        [Summary
        ("Restarts a VRising Container")]
        public async Task StateAsync()
        {
            await Context.Channel.SendMessageAsync("Getting current server status ...");
            var result = await aciService.ExecuteCommand(ContainerCommand.State);
            await Context.Channel.SendMessageAsync(result);
        }
    }
}
