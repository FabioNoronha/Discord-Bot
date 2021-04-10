using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord;
using DiscordBot.Core.ServerLocationAccount;

namespace DiscordBot
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += AnnounceJoinedUser;
            _client.UserLeft += AnnounceLeavingUser;
            //await _client.MessagesBulkDeleted += BulkDelete;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);
            int argPos = 0;

            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                if (context.User.IsBot == false)
                {
                    var result = await _service.ExecuteAsync(context, argPos, null);
                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }                    
                }
            }

            if (context.User.IsBot == false)
            {
                var account = UserAccountsPlural.GetAccount(context.User);
                account.XP += 1;

                if (s.ToString() == "zenred" || s.ToString() == "Zenred")
                {
                    await s.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ZENRED"));
                }

                if (s.ToString() == "e" || s.ToString() == "E")
                {
                    await s.Channel.SendMessageAsync("than.");
                }
            }
        }

        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var guild = user.Guild;
            var greeting = GreetingsPlural.GetServer(guild);

            if (greeting.GreetingTurned == true)
            {
                var channel = greeting.GreetingChannelID;
                var chnl = _client.GetChannel(channel) as IMessageChannel;

                string result = greeting.GreetingMessage.Replace("{user}", user.Mention);

                await chnl.SendMessageAsync(result);
            }
        }

        public async Task AnnounceLeavingUser(SocketGuildUser user)
        {
            var guild = user.Guild;
            var goodbye = GreetingsPlural.GetServer(guild);

            if (goodbye.GoodbyeTurned == true)
            {
                var channel = goodbye.GoodbyeChannelID;
                var chnl = _client.GetChannel(channel) as IMessageChannel;

                string result = goodbye.GoodbyeMessage.Replace("{user}", user.Username);

                await chnl.SendMessageAsync(result);
            }
        }

        public async Task BulkDelete(SocketMessage s)
        {
            await s.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BULK_DELETE"));
        }
    }
}
