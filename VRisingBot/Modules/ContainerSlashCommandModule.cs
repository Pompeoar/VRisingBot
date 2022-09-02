using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRisingBot.Services;

namespace VRisingBot.Modules
{
    public class ContainerSlashCommandModule : InteractionModuleBase<SocketInteractionContext>
    {        
        private CommandHandler handler;
        
        public ContainerSlashCommandModule(CommandHandler handler)
        {
            this.handler = handler;
        }

        // our first /command!
        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question)
        {
            // create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");

            // get the answer
            var answer = replies[new Random().Next(replies.Count - 1)];

            // reply with the answer
            await RespondAsync($"You asked: [**{question}**], and your answer is: [**{answer}**]");
        }
    }
}
