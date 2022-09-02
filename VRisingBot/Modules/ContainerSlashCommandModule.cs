using Discord.Interactions;
using System.Threading.Tasks;
using System.Threading;
using VRisingBot.Services;

namespace VRisingBot.Modules
{
    public class ContainerSlashCommandModule : InteractionModuleBase<SocketInteractionContext>
    {        
        private CommandHandler handler;
        private List<string> replies;
        private Random random;

        public ContainerSlashCommandModule(CommandHandler handler)
        {
            this.handler = handler;
            replies = CreatePossibilities();
            random = new Random();   
        }

        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question) => 
            await RespondAsync($"{replies[random.Next(replies.Count - 1)]}");

        private static List<string> CreatePossibilities()
        {
            // create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("Yes!");
            replies.Add("Maybe later.");
            replies.Add("Trust me, you don't want to know.");
            replies.Add("It's hard to say.");
            replies.Add("Don’t bet on it.");
            replies.Add("It is known.");
            replies.Add("So say we all.");
            replies.Add("Meh.");
            replies.Add("Probably.");
            replies.Add("No.");
            replies.Add("It doesn't look good.");
            replies.Add("Absolutely.");
            replies.Add("Stormy weather ahead.");
            replies.Add("I am certain of it.");
            replies.Add("Definitely.");
            replies.Add("Yes.Break out the champagne.");
            replies.Add("Even a blind squirrel finds a nut sometimes.");
            replies.Add("So let it be written.So let it be done.");
            replies.Add("Someone has forgotten the face of their father.");
            return replies;
        }
    }
}
