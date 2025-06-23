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
        [SlashCommand("Help", "Fergus shows you the help menu!")]
        public async Task Help(InteractionContext ctx)
        {
            var dmChannel = await ctx.Member.CreateDmChannelAsync();
            var response = new DSharpPlus.Entities.DiscordMessageBuilder()
            .WithContent($"Hello {ctx.User.Mention}! Here are the commands you can use:\n" +
                 "`/ping` - Pings Fergus and gets a Pong back!\n" +
                 "`/flip-a-coin` - Flips a coin for you!\n" +
                 "`/random-number <max>` - Generates a random number from 0 to your specified max!\n" +
                 "`/hotseat` - Picks a random user from the Voice Channel to be in the hotseat!\n" +
                 "`/liars-dice` - Rolls a set of 5 dice for you!\n" +
                 "`/grab-suggestions <user>` - Grabs all Spotify links sent by a user in The Spotify Channel!");

            await dmChannel.SendMessageAsync(response);

            await ctx.CreateResponseAsync(
                DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                    .WithContent("I've sent you a DM with the help menu! 📬")
                    .AsEphemeral(true) // This makes the message only visible to the sender
            ).ConfigureAwait(false);
        }
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

        private static readonly Random rng = new Random();

        [SlashCommand("Flip-A-Coin", "Fergus flips a penny for you!")]
        public async Task CoinFlip(InteractionContext ctx)
        {
            var coin = rng.Next(0, 2);
            string result;
            if (coin == 0)
            {
                result = "Heads";
            }
            else
            {
                result = "Tails";
            }
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"The coin landed on: {result}!");
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }

        [SlashCommand("Random-Number", "Fergus gives you a random number from 0 to your choosing!")]
        public async Task RandomNumber(InteractionContext ctx, [Option("max", "The maximum number you want to generate.")] double max)
        {
            // Use the existing static rng instance
            var number = rng.Next(0, (int)max+1);
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"{ctx.User.Mention} your random number is: {number}! :confetti_ball:");
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }

        [SlashCommand ("Hotseat", "Fergus picks a random user from the Voice Channel to be in the hotseat!")]
        public async Task Hotseat(InteractionContext ctx)
        {
            var voiceState = ctx.Member.VoiceState;
            if (voiceState == null || voiceState.Channel == null)
            {
                await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent
                ($"{ctx.User.Mention}...i can only see you...alone...Join a VC to use Hotseat")).ConfigureAwait(false);
                return;
            }

            var members = voiceState.Channel.Users;
            var member = members[rng.Next(0, members.Count)];
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent($"The hotseat goes to: {member.Mention}!")).ConfigureAwait(false);
        }

        [SlashCommand("Liars-Dice", "Fergus rolls a set of 5 dice for you!")]
        public async Task LiarsDice(InteractionContext ctx)
        {
            var diceRolls = new List<int>();
            for (int i = 0; i < 5; i++)
            {
            diceRolls.Add(rng.Next(1, 7));
            }
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"Here are your dice rolls:\n{string.Join(" - ", diceRolls)}")
            .AsEphemeral(true); // Make the response visible only to the user who invoked the command
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }

        [SlashCommand("Grab-Suggestions", "Fergus grabs all the Spotify Links sent in The Spotify Channel by a certain user!")]
        public async Task GrabSuggestions(InteractionContext ctx, [Option("user", "The user you want to grab the links from.")] DSharpPlus.Entities.DiscordUser user)
        {
            var channel = ctx.Guild.GetChannel(1276012121502519390);
            var messages = await channel.GetMessagesAsync();
            var links = new List<string>();
            foreach (var message in messages)
            {
                if (message.Author == user && message.Content.Contains("https://open.spotify.com/"))
                {
                    links.Add(message.Content);
                }
            }
            var response = new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent($"Here are the Spotify links from {user.Mention}:\n{string.Join("\n", links)}")
            .AsEphemeral(true);
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, response)
            .ConfigureAwait(false);
        }
        [SlashCommand("Split Call", "Fergus will split the calls randomly across 2 channels!")]
        public async Task SplitCall(InteractionContext ctx)
        {
            // Only allow command in channel 1386665681944313889
            if (ctx.Channel.Id != 1386665681944313889)
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("This command can only be used in the designated channel.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Only allow users with role 1204422815655137280
            if (!ctx.Member.Roles.Any(r => r.Id == 1204422815655137280))
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("You do not have permission to use this command.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            var voiceState = ctx.Member.VoiceState;
            if (voiceState == null || voiceState.Channel == null)
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent($"{ctx.User.Mention}...i can only see you...alone...*yikes* Join a VC to use Split Call"))
                .ConfigureAwait(false);
            return;
            }

            var members = voiceState.Channel.Users;
            var memberList = new List<DSharpPlus.Entities.DiscordMember>(members);

            // Identify console users (bot/self or users with no VoiceState)
            var exemptMembers = new List<DSharpPlus.Entities.DiscordMember>();
            foreach (var member in memberList.ToList())
            {
            // Exempt bots and users with no VoiceState (console users)
            if (member.IsBot || member.VoiceState == null || member.VoiceState.Channel == null)
            {
                exemptMembers.Add(member);
                memberList.Remove(member);
            }
            }

            // If there are still members left, allow manual exemption via console input (if running interactively)
    #if DEBUG
            if (memberList.Count > 0 && System.Console.IsInputRedirected == false)
            {
            System.Console.WriteLine("Current members in VC:");
            for (int i = 0; i < memberList.Count; i++)
            {
                System.Console.WriteLine($"{i}: {memberList[i].Username}#{memberList[i].Discriminator}");
            }
            System.Console.WriteLine("Enter comma-separated indices of users to exempt from move (or leave blank):");
            var input = System.Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                var indices = input.Split(',').Select(s => s.Trim()).Where(s => int.TryParse(s, out _)).Select(int.Parse).Distinct().ToList();
                foreach (var idx in indices.OrderByDescending(x => x))
                {
                if (idx >= 0 && idx < memberList.Count)
                {
                    exemptMembers.Add(memberList[idx]);
                    memberList.RemoveAt(idx);
                }
                }
            }
            }
    #endif

            // If majority is exempt, do not allow move
            int totalUsers = memberList.Count + exemptMembers.Count;
            if (memberList.Count == 0 || memberList.Count < exemptMembers.Count)
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("Too many users are exempt from move. Cannot split call.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Shuffle the list of members
            for (int i = memberList.Count - 1; i > 0; i--)
            {
            int j = rng.Next(0, i + 1);
            var temp = memberList[i];
            memberList[i] = memberList[j];
            memberList[j] = temp;
            }

            // Split as evenly as possible
            int halfCount = memberList.Count / 2;
            int remainder = memberList.Count % 2;
            var group1 = memberList.GetRange(0, halfCount + remainder); // If odd, group1 gets the extra
            var group2 = memberList.GetRange(halfCount + remainder, memberList.Count - (halfCount + remainder));

            // Use the original channel and the specified other channel
            var channel1 = voiceState.Channel;
            var channel2 = ctx.Guild.GetChannel(1386665718199877707);

            // Move the members to their respective channels
            foreach (var member in group1)
            {
            try { await member.ModifyAsync(x => x.VoiceChannel = channel1).ConfigureAwait(false); }
            catch { /* ignore move errors */ }
            }
            foreach (var member in group2)
            {
            try { await member.ModifyAsync(x => x.VoiceChannel = channel2).ConfigureAwait(false); }
            catch { /* ignore move errors */ }
            }

            // Build a table of assignments
            var sb = new StringBuilder();
            sb.AppendLine("**Split Call Results:**");
            sb.AppendLine("| Channel | Members |");
            sb.AppendLine("|---------|---------|");
            sb.AppendLine($"| {channel1.Mention} | {string.Join(", ", group1.Select(m => m.Mention))} |");
            sb.AppendLine($"| {channel2.Mention} | {string.Join(", ", group2.Select(m => m.Mention))} |");
            if (exemptMembers.Count > 0)
            {
            sb.AppendLine();
            sb.AppendLine("**Exempt from move:**");
            sb.AppendLine(string.Join(", ", exemptMembers.Select(m => m.Mention)));
            }

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
            new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent(sb.ToString()))
            .ConfigureAwait(false);
        }
        [SlashCommand("Match Finished", "Fergus will announce the match finished in the channel!")]
        public async Task MatchFinished(
            InteractionContext ctx,
            [Option("winning_team", "The winning team")] string winningTeam // Dropdown option
        )
        {
            // Allow command only in channels 1386665681944313889 or 1386665718199877707
            var allowedChannels = new ulong[] { 1386665681944313889, 1386665718199877707 };
            if (!allowedChannels.Contains(ctx.Channel.Id))
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("This command can only be used in the designated channels.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Only allow users with role 1204422815655137280
            if (!ctx.Member.Roles.Any(r => r.Id == 1204422815655137280))
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("You do not have permission to use this command.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Get the voice channel associated with the text channel
            var voiceChannelId = ctx.Channel.Id == 1386665681944313889
            ? 1386665681944313889UL
            : 1386665718199877707UL;
            var voiceChannel = ctx.Guild.GetChannel(voiceChannelId);

            // Get members in the voice channel
            var members = voiceChannel.Users.ToList();
            if (members.Count == 0)
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("No users found in the associated voice channel.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Split members into two teams (same as SplitCall logic)
            var memberList = new List<DSharpPlus.Entities.DiscordMember>(members);
            int halfCount = memberList.Count / 2;
            var group1 = memberList.GetRange(0, halfCount);
            var group2 = memberList.GetRange(halfCount, memberList.Count - halfCount);

            // Determine winning group based on dropdown value
            List<DSharpPlus.Entities.DiscordMember> winners;
            List<DSharpPlus.Entities.DiscordMember> losers;
            string teamName;
            if (string.Equals(winningTeam, "Team A", StringComparison.OrdinalIgnoreCase))
            {
            winners = group1;
            losers = group2;
            teamName = "Team A";
            }
            else if (string.Equals(winningTeam, "Team B", StringComparison.OrdinalIgnoreCase))
            {
            winners = group2;
            losers = group1;
            teamName = "Team B";
            }
            else
            {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
                new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent("Invalid team selection. Please pick either Team A or Team B.")
                .AsEphemeral(true)).ConfigureAwait(false);
            return;
            }

            // Load scores from file (simple JSON dictionary)
            var scoreFile = "match_scores.json";
            Dictionary<ulong, int> scores = new Dictionary<ulong, int>();
            if (System.IO.File.Exists(scoreFile))
            {
            var json = await System.IO.File.ReadAllTextAsync(scoreFile);
            scores = System.Text.Json.JsonSerializer.Deserialize<Dictionary<ulong, int>>(json) ?? new Dictionary<ulong, int>();
            }

            // Add +1 to each winner
            foreach (var member in winners)
            {
            if (scores.ContainsKey(member.Id))
                scores[member.Id]++;
            else
                scores[member.Id] = 1;
            }

            // Save scores
            var updatedJson = System.Text.Json.JsonSerializer.Serialize(scores);
            await System.IO.File.WriteAllTextAsync(scoreFile, updatedJson);

            // Build result table
            var sb = new StringBuilder();
            sb.AppendLine($"Match Finished! :trophy:");
            sb.AppendLine($"Winning Team: {teamName}");
            sb.AppendLine();
            sb.AppendLine("| Player | Total Points | Change |");
            sb.AppendLine("|--------|--------------|--------|");

            foreach (var member in winners)
            {
            int total = scores.TryGetValue(member.Id, out var val) ? val : 0;
            sb.AppendLine($"| {member.Mention} | {total} | +1 |");
            }
            foreach (var member in losers)
            {
            int total = scores.TryGetValue(member.Id, out var val) ? val : 0;
            sb.AppendLine($"| {member.Mention} | {total} | 0 |");
            }

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource,
            new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent(sb.ToString())
                .AsEphemeral(true)).ConfigureAwait(false);
        }

        
        

    }
}