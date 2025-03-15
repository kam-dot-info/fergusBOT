using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace fergusBOT.Commands
{
    public class basicSLASHCOMMANDS : ApplicationCommandModule
    {
        [SlashCommand("Ping", "Pokes Fergus and he returns a Pong!")]
        public async Task Ping(InteractionContext ctx)
        {
            var latency = ctx.Client.Ping;
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"Here's the Pong to your Ping {ctx.User.Mention} :ping_pong::heart:\n" +
                     $"(P.S. It took me {latency}ms to process that :wink:)");
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }

        [SlashCommand("Flip-A-Coin", "Fergus flips a penny for you!")]
        public async Task CoinFlip(InteractionContext ctx)
        {
            var rng = new Random();
            var coin = rng.Next(0, 1);
            if (coin == 0)
            {
                var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent($"The coin landed on: Heads!");
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
                .ConfigureAwait(false);
                return;
            }
            else
            {
                var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent($"The coin landed on: Tails!");
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
                .ConfigureAwait(false);
                return;
            }
        }

        [SlashCommand("Random-Number", "Fergus gives you a random number from 0 to your choosing!")]
        public async Task RandomNumber(InteractionContext ctx, [Option("max", "The maximum number you want to generate.")] double max)
        {
            var rng = new Random();
            var number = rng.Next(0, (int)max);
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"{ctx.User.Mention} your random number is: {number}! :confetti_ball:");
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }
        

    }
}