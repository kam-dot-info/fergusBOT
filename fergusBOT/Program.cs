// See https://aka.ms/new-console-template for more information
using System;
using System.Drawing;
using DSharpPlus; //DSharpPlus references
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using fergusBOT.config;


namespace fergusBOT
{
    internal class Program
    {
        private static DiscordClient Client { get; set; } = null!;
      
        private static SlashCommandsExtension SlashCommands { get; set; } = null!;

        static async Task Main(string[] args)
        {
            Console.WriteLine("STARTING BOT");
            var jsonReader = new jsonREADER();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += Client_Ready;

            var slashConfig = new SlashCommandsConfiguration()
            {
                Services = null
            };

            SlashCommands = Client.UseSlashCommands(slashConfig);

            SlashCommands.RegisterCommands<Commands.basicSLASHCOMMANDS>();

            await Client.UpdateStatusAsync(new DiscordActivity("in the boot", ActivityType.Playing));

            await Client.ConnectAsync();
            await Task.Delay(Timeout.Infinite);
            
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.WriteLine("fergusBot has finished starting up");
            return Task.CompletedTask;
        }

    }

}


