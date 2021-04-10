using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.ServerLocationAccount;
using NReco.ImageGenerator;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace DiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private bool IsUserChallenged(SocketGuildUser user)
        {
            string targetRoleName = "Actual Braindead";
            var result = from r in user.Guild.Roles where r.Name == targetRoleName select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        private bool LikedByYuriBot(SocketGuildUser user)
        {
            string targetRoleName = "Liked by YuriBot";
            var result = from r in user.Guild.Roles where r.Name == targetRoleName select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }

        private bool DoesUserHaveAdmin(SocketGuildUser user)
        {
            return user.GuildPermissions.Administrator;
        }

        public static int bullets = 6;

        //private ITextChannel TargetChannel(SocketGuildUser user)
        //{
        //    string target = SetTarget();
        //    var result = from r in user.Guild.Channels where r.Name == target select r.Id;
        //}

        //private static bool isServerUp;

        public async Task GetAlbumLength(string albumName, string album)
        {
            var client = new RestClient($"https://api.imgur.com/3/album/{album}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ALBUM_LENGTH", albumName, dataObject.data.images_count));
        }

        //Index        
        string[] TopicIndex =
        {
             "What is your guys' favorite animal?",
             "What is your guys' favorite food?",
             "What is your guys' favorite game? And what's your favorite thing about it?",
             "If someday you found 100€ on the ground what would you do?",
             "How was your last birthday?",
             "Does anyone have any pet peeves?",
             "What is the last dream you remember?",
             "Name one weird thing about yourself.",
             "What is something you would like to try?",
             "What is the website you visit most often?",
             "What pet would you like to have?",
             "If you could have any animal as a pet, which would you pick?",
             "What is your favorite kind of music?",
             "What is your ideal place to live?",
             "Just cats :) meooow!",
             "What is your favorite ben 10 transformation?"
        };

        string[] StatusIndex =
        {
            "I kinda hate you all honestly.",
            "You guys are really cool!",
            "Leave me alone omfg.",
            "Why do things that happen to stupid people keep happening to me...",
            "Always remember happy day",
            "with your heart <3",
            "Ohno I forgot to set up a status uhhhh uhhh penis.",
            "a ukulele with pichu."
        };
        //End of Index

        [Command("Help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 0, 200));

            if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
            {
                embed.WithTitle(Utilities.GetFormattedAlert("NICE_HELP_REQUEST"));
            }
            else
            {
                embed.WithTitle(Utilities.GetFormattedAlert("HELP_REQUEST"));
            }

            embed.WithDescription(Utilities.GetFormattedAlert("HELP", "``-> ", " "));

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("Status")]
        public async Task Status([Remainder]string message = "")
        {
            if (message == null || message == "")
            {
                Random r = new Random();
                string statusfinal = StatusIndex[r.Next(StatusIndex.Length)];

                await Context.Client.SetGameAsync(statusfinal);
                return;
            }

            await Context.Client.SetGameAsync(message);
        }

        [Command("Set Up Greeting")]
        public async Task SetupGreeting(SocketGuildChannel channel, [Remainder]string message = "")
        {
            if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
            {
                SocketGuild server = Context.Guild;
                var greeting = GreetingsPlural.GetServer(server);

                if (channel != null)
                {
                    greeting.GreetingChannelID = channel.Id;
                }
                else
                {
                    greeting.GoodbyeChannelID = Context.Guild.DefaultChannel.Id;
                }

                greeting.GreetingTurned = true;
                greeting.GreetingMessage = message;

                GreetingsPlural.SaveServer();

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING"));
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                }
            }
        }

        [Command("Set Up Greeting")]
        public async Task Setup([Remainder]string message = "")
        {
            await SetupGreeting(null, message);
        }

        [Command("Turn off Greeting")]
        public async Task GreetingTurnOff()
        {
            SocketGuild server = Context.Guild;
            var greeting = GreetingsPlural.GetServer(server);

            if (greeting.GreetingTurned == true)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    greeting.GreetingTurned = false;
                    GreetingsPlural.SaveServer();
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_TURN_OFF"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_TURN_OFF"));
                    }
                }
                else
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                    }
                }
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_ALREADY_OFF"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_ALREADY_OFF"));
                }
            }
        }

        [Command("Turn on Greeting")]
        public async Task GreetingTurnOn()
        {
            SocketGuild server = Context.Guild;
            var greeting = GreetingsPlural.GetServer(server);

            if (greeting.GreetingTurned == false)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    greeting.GreetingTurned = true;
                    GreetingsPlural.SaveServer();
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_TURN_ON"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_TURN_ON"));
                    }
                }
                else
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                    }
                }
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_ALREADY_ON"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_ALREADY_ON"));
                }
            }
        }

        [Command("Set up Goodbye")]
        public async Task GoodbyeMessage(SocketGuildChannel channel, [Remainder] string message = "")
        {
            if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
            {
                SocketGuild server = Context.Guild;
                var greeting = GreetingsPlural.GetServer(server);

                if (channel != null)
                {
                    greeting.GoodbyeChannelID = channel.Id;
                }
                else
                {
                    greeting.GoodbyeChannelID = Context.Guild.DefaultChannel.Id;
                }

                greeting.GoodbyeTurned = true;
                greeting.GoodbyeMessage = message;

                GreetingsPlural.SaveServer();

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GOODBYE"));
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                }
            }
        }

        [Command("Setup Goodbye Message")]
        public async Task GoodbyeMessage([Remainder] string message = "")
        {
            await GoodbyeMessage(null, message);
        }

        [Command("Turn off Goodbye Message")]
        public async Task GoodbyeTurnOff()
        {
            SocketGuild server = Context.Guild;
            var goodbye = GreetingsPlural.GetServer(server);

            if (goodbye.GoodbyeTurned == true)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    goodbye.GoodbyeTurned = false;
                    GreetingsPlural.SaveServer();

                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GOODBYE_TURN_OFF"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GOODBYE_TURN_OFF"));
                    }
                }
                else
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                    }
                }
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GOODBYE_ALREADY_OFF"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GOODBYE_ALREADY_OFF"));
                }
            }
        }

        [Command("Turn on Goodbye Message")]
        public async Task GoodbyeTurnOn()
        {
            SocketGuild server = Context.Guild;
            var goodbye = GreetingsPlural.GetServer(server);

            if (goodbye.GoodbyeTurned == false)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    goodbye.GoodbyeTurned = true;
                    GreetingsPlural.SaveServer();

                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GOODBYE_TURN_ON"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GOODBYE_TURN_ON"));
                    }
                }
                else
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING_NO_PERM"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GREETING_NO_PERM"));
                    }
                }
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GOODBYE_ALREADY_ON"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GOODBYE_ALREADY_ON"));
                }
            }
        }

        [Command("Like")]
        public async Task Like([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            var account = UserAccountsPlural.GetAccount(target);

            if (account.isLiked == false)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    account.isLiked = true;
                    UserAccountsPlural.SaveAccounts();

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("LIKE", target.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NO_PERM_LIKE"));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ALREADY_LIKED", target.Username));
            }
        }

        [Command("Dislike")]
        public async Task Dislike([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            var account = UserAccountsPlural.GetAccount(target);

            if (account.isLiked == true)
            {
                if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
                {
                    account.isLiked = false;
                    UserAccountsPlural.SaveAccounts();

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DISLIKE", target.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NO_PERM_DISLIKE", target.Username));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ALREADY_DISLIKED", target.Username));
            }
        }

        [Command("Are they liked")]
        public async Task AreTheyLiked([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            var account = UserAccountsPlural.GetAccount(target);

            if (account.isLiked == true)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("IS_LIKED", target.Username, target.Mention));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("IS_DISLIKED", target.Username));
            }
        }

        [Command("Is he liked")]
        public async Task AreTheyLiked2([Remainder]string arg = "")
        {
            await AreTheyLiked(arg);
        }

        [Command("Is she liked")]
        public async Task AreTheyLiked3([Remainder]string arg = "")
        {
            await AreTheyLiked(arg);
        }

        [Command("Server")]
        public async Task ServerAddress()
        {
            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SERVER_ADDRESS_REQUEST", Utilities.GetFormattedAlert("SERVER_ADDRESS")));
        }

        //[Command("The server is down")]
        //public async Task ServerDown()
        //{
        //    isServerUp = false;
        //    if (!LikedByYuriBot((SocketGuildUser)Context.User))
        //    {
        //        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SERVER_TURN_OFF", Context.User.Username));
        //        return;
        //    }
        //
        //    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_SERVER_TURN_OFF", Context.User.Username));
        //}

        //[Command("The server is up")]
        //public async Task ServerUp()
        //{
        //    isServerUp = true;
        //    if (!LikedByYuriBot((SocketGuildUser)Context.User))
        //    {
        //        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SERVER_TURN_ON", Context.User.Username));
        //        return;
        //    }
        //
        //    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_SERVER_TURN_ON", Context.User.Username));
        //}        

        [Command("Is the server up")]
        public async Task ServerCheck()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://api.mcsrvstat.us/2/" + Utilities.GetFormattedAlert("SERVER_ADDRESS"));
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string isServerUp = dataObject.online.ToString();

            if (isServerUp == "False")
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_SERVER_OFF", Context.User.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SERVER_OFF", Context.User.Username));
            }

            if (isServerUp == "True")
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_SERVER_ON", Context.User.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SERVER_ON", Context.User.Username));
            }
        }

        [Command("server up?")]
        public async Task ServerCheck2()
        {
            await ServerCheck();
        }

        [Command("Number of players")]
        public async Task NumberOfPlayers()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://api.mcsrvstat.us/2/" + Utilities.GetFormattedAlert("SERVER_ADDRESS"));
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string isServerUp = dataObject.online.ToString();

            if (isServerUp == "True")
            {
                string players = dataObject.players.online.ToString();
                
                if (players == "0")
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS_EMPTY"));
                    return;
                }
                else if (players == "1")
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS_ONE"));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS", players));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS_EMPTY"));
            }
        }

        [Command("Player Number")]
        public async Task NumberOfPlayers2()
        {
            await NumberOfPlayers();
        }

        [Command("Player list")]
        public async Task PlayerList()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://api.mcsrvstat.us/2/" + Utilities.GetFormattedAlert("SERVER_ADDRESS"));
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string isServerUp = dataObject.online.ToString();

            if (isServerUp == "True")
            {
                string players = dataObject.players.online.ToString();

                if (players == "0")
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS_EMPTY"));
                    return;
                }

                var embed = new EmbedBuilder();
                embed.WithColor(new Color(255, 0, 200));
                embed.WithTitle("Player list: ");

                for (int i = 0; i < Int64.Parse(players); i++)
                {
                    string playerList = dataObject.players.list[i].ToString();
                    embed.AddField("Player " + (i + 1) + ":", playerList);
                }

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NUMBER_OF_PLAYERS_EMPTY"));
            }
        }

        [Command("PlayerList")]
        public async Task PlayerList2()
        {
            await PlayerList();
        }

        [Command("Players List")]
        public async Task PlayerList3()
        {
            await PlayerList();
        }

        [Command("Player")]
        public async Task PlayerList4()
        {
            await PlayerList();
        }

        [Command("Players")]
        public async Task PlayerList5()
        {
            await PlayerList();
        }

        [Command("My XP")]
        public async Task MyXP([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            var account = UserAccountsPlural.GetAccount(target);

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("XP_SHOWCASE", target.Mention, account.XP));
        }

        [Command("XP")]
        public async Task XP([Remainder]string arg = "")
        {
            await MyXP(arg);
        }

        [Command("Add XP")]
        public async Task AddXP(uint ammount, [Remainder]string arg = "")
        {
            var giver = UserAccountsPlural.GetAccount(Context.User);
            if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
            {
                SocketUser target = null;
                var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
                target = (mentionedUser == null) ? Context.User : mentionedUser;

                var account = UserAccountsPlural.GetAccount(target);
                account.XP += ammount;
                UserAccountsPlural.SaveAccounts();

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("XP_ADDED", target.Mention, ammount));
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_NO_PERM_XP"));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NO_PERM_XP"));
            }
        }

        [Command("Say")]
        public async Task Say(ITextChannel targetChannel, [Remainder]string message)
        {
            if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
            {
                await targetChannel.SendMessageAsync(message);
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_ECHO_NO_PERM", message));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ECHO_NO_PERM", Context.User.Username));
                }
            }
        }

        [Command("Speak")]
        public async Task Speak(ITextChannel targetChannel, [Remainder]string message)
        {
            await Say(targetChannel, message);
        }

        [Command("Echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Message sent by: " + Context.User.Username);
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 200));
            embed.WithCurrentTimestamp();
            embed.WithImageUrl("https://imgur.com/S9iCDrh.png");

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("Hello")]
        public async Task Greeting()
        {
            if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
            {
                string name = Context.User.Username;

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GREETING", name));
                return;
            }

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            await dmChannel.SendMessageAsync(Utilities.GetFormattedAlert("RUDE_GREETING"));
            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("RUDE_GREETING2"));
        }

        [Command("Gay")]
        public async Task Insult()
        {
            if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_GAY"));
                return;
            }

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("GAY"));
        }

        [Command("wyd wya")]
        public async Task Retard()
        {
            string name = Context.User.Username;

            if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_WDY_WYA", name));
                return;
            }

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("WDY_WYA", name));
        }

        [Command("Topic")]
        public async Task GiveATopic(ITextChannel channel)
        {
            Random r = new Random();
            string selectedTopic = TopicIndex[r.Next(0, TopicIndex.Length)];

            if (DoesUserHaveAdmin((SocketGuildUser)Context.User) == true)
            {
                await Say(channel, Utilities.GetFormattedAlert("TOPIC_PICKER", selectedTopic));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("TOPIC_PICKER", selectedTopic));
            }
        }

        [Command("Topic")]
        public async Task Topic()
        {
            Random r = new Random();
            string selectedTopic = TopicIndex[r.Next(0, TopicIndex.Length)];

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("TOPIC_PICKER", selectedTopic));
        }

        [Command("Choose")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            char[] trimChars = { '*', '@', ' ' };            

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            selection.Trim(trimChars);
            
            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("CHOICE_RESULT", selection));
            DataStorage.AddPairToStorage(Context.User.Username + DateTime.Now.ToShortTimeString(), selection);
        }

        [Command("Hilda")]
        public async Task Hilda(int HildaNumber)
        {
            var client = new RestClient("https://api.imgur.com/3/album/zDxGzYS/images");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            string link;

            if (HildaNumber <= 0 || HildaNumber > dataObject.data.Count)
            {
                Random r = new Random();
                int randomNumber = r.Next(0, dataObject.data.Count);

                link = dataObject.data[randomNumber].link;
            }
            else
            {
                HildaNumber = HildaNumber - 1;
                link = dataObject.data[HildaNumber].link;
            }

            var HildaEmbed = new EmbedBuilder();
            HildaEmbed.WithTitle("Hilda Hilda Hilda");
            HildaEmbed.WithDescription("[Click here to see the full Hilda album.](https://imgur.com/a/zDxGzYS)");
            HildaEmbed.WithColor(new Color(255, 0, 200));
            HildaEmbed.WithImageUrl(link);
            HildaEmbed.WithCurrentTimestamp();
            HildaEmbed.WithFooter("Hilda requested by: " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, HildaEmbed.Build());
        }

        [Command("Hilda")]
        public async Task Hilda2()
        {
            await Hilda(0);
        }

        [Command("Hilda Total")]
        public async Task HildaTotal()
        {
            await GetAlbumLength("Hilda", "zDxGzYS");
        }

        [Command("Number of Hilda")]
        public async Task HildaTotal2()
        {
            await HildaTotal();
        }

        [Command("Hilda Number")]
        public async Task HildaTotal3()
        {
            await HildaTotal();
        }

        [Command("SadCat")]
        public async Task SadCat(int SadCatNumber)
        {
            var client = new RestClient("https://api.imgur.com/3/album/WaQaeKb/images");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            string link;

            if (SadCatNumber <= 0 || SadCatNumber > dataObject.data.Count)
            {
                Random r = new Random();
                int randomNumber = r.Next(0, dataObject.data.Count);

                link = dataObject.data[randomNumber].link;
            }
            else
            {
                SadCatNumber = SadCatNumber - 1;
                link = dataObject.data[SadCatNumber].link;
            }

            var SadCatEmbed = new EmbedBuilder();
            SadCatEmbed.WithTitle("The sad is cat, I mean cat is sad");
            SadCatEmbed.WithDescription("[Click here to see the full Sad Cat album.](https://imgur.com/a/WaQaeKb)");
            SadCatEmbed.WithColor(new Color(255, 0, 200));
            SadCatEmbed.WithImageUrl(link);
            SadCatEmbed.WithCurrentTimestamp();
            SadCatEmbed.WithFooter("Cat requested by: " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, SadCatEmbed.Build());
        }

        [Command("SadCat")]
        public async Task SadCat2()
        {
            await SadCat(0);
        }

        [Command("SadCats")]
        public async Task SadCat3()
        {
            await SadCat(0);
        }

        [Command("SadCats")]
        public async Task SadCat4(int SadCatNumber)
        {
            await SadCat(SadCatNumber);
        }

        [Command("Sad Cats")]
        public async Task SadCat5()
        {
            await SadCat(0);
        }

        [Command("Sad Cats")]
        public async Task SadCat6(int SadCatNumber)
        {
            await SadCat(SadCatNumber);
        }

        [Command("Sad Cat")]
        public async Task SadCat7()
        {
            await SadCat(0);
        }        

        [Command("Sad Cat")]
        public async Task SadCat8(int SadCatNumber)
        {
            await SadCat(SadCatNumber);
        }

        [Command("Sad cat total")]
        public async Task SadCatTotal()
        {
            await GetAlbumLength("SadCat", "WaQaeKb");            
        }

        [Command("SadCat Total")]
        public async Task SadCatTotal2()
        {
            await SadCatTotal();
        }

        [Command("Number of Sad Cat")]
        public async Task SadCatTotal3()
        {
            await SadCatTotal();
        }

        [Command("Number of Sad Cats")]
        public async Task SadCatTotal4()
        {
            await SadCatTotal();
        }

        [Command("Number of SadCat")]
        public async Task SadCatTotal5()
        {
            await SadCatTotal();
        }

        [Command("Number of SadCats")]
        public async Task SadCatTotal6()
        {
            await SadCatTotal();
        }

        [Command("SadCats Number")]
        public async Task SadCatTotal7()
        {
            await SadCatTotal();
        }

        [Command("Sad Cats Number")]
        public async Task SadCatTotal8()
        {
            await SadCatTotal();
        }

        [Command("SadCat Number")]
        public async Task SadCatTotal9()
        {
            await SadCatTotal();
        }

        [Command("SadCats Number")]
        public async Task SadCatTotal10()
        {
            await SadCatTotal();
        }

        [Command("Ceniza")]
        public async Task Ceniza(int CenizaNumber)
        {
            var client = new RestClient("https://api.imgur.com/3/album/z6ikZIC/images");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            string link;

            if (CenizaNumber <= 0 || CenizaNumber > dataObject.data.Count)
            {
                Random r = new Random();
                int randomNumber = r.Next(0, dataObject.data.Count);

                link = dataObject.data[randomNumber].link;
            }
            else
            {
                CenizaNumber = CenizaNumber - 1;
                link = dataObject.data[CenizaNumber].link;
            }

            var CenizaEmbed = new EmbedBuilder();
            CenizaEmbed.WithTitle("Ceniza has been patted by: " + Context.User.Username);
            CenizaEmbed.WithDescription("[Click here to see the full Ceniza album.](https://imgur.com/a/z6ikZIC)");
            CenizaEmbed.WithColor(new Color(255, 0, 200));
            CenizaEmbed.WithImageUrl(link);
            CenizaEmbed.WithCurrentTimestamp();
            CenizaEmbed.WithFooter("Ceniza thanks " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, CenizaEmbed.Build());
        }

        [Command("Pat Ceniza")]
        public async Task Ceniza2(int CenizaNumber)
        {
            await Ceniza(CenizaNumber);
        }

        [Command("Ceniza")]
        public async Task Ceniza()
        {
            await Ceniza(0);
        }

        [Command("Pat Ceniza")]
        public async Task Ceniza2()
        {
            await Ceniza(0);
        }

        [Command("Ceniza Total")]
        public async Task CenizaTotal()
        {
            await GetAlbumLength("Ceniza", "z6ikZIC");
        }

        [Command("Number of Ceniza")]
        public async Task CenizaTotal2()
        {
            await CenizaTotal();
        }

        [Command("ePet")]
        public async Task ePet(int eCatNumber)
        {
            var client = new RestClient("https://api.imgur.com/3/album/RZfZr1P/images");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            string link;

            if (eCatNumber <= 0 || eCatNumber > dataObject.data.Count)
            {
                Random r = new Random();
                int randomNumber = r.Next(0, dataObject.data.Count);

                link = dataObject.data[randomNumber].link;
            }
            else
            {
                eCatNumber = eCatNumber - 1;
                link = dataObject.data[eCatNumber].link;
            }

            var CatEmbed = new EmbedBuilder();
            CatEmbed.WithTitle("This is one of Esthers cute pets~");
            CatEmbed.WithDescription("[Click here to see the full Esthers pets album.](https://imgur.com/a/RZfZr1P)");
            CatEmbed.WithColor(new Color(255, 0, 200));
            CatEmbed.WithImageUrl(link);
            CatEmbed.WithCurrentTimestamp();
            CatEmbed.WithFooter("Esther thanks " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, CatEmbed.Build());
        }

        [Command("ePet")]
        public async Task ePet2()
        {
            await ePet(0);
        }

        [Command("ePet Total")]
        public async Task ePetTotal()
        {
            await GetAlbumLength("Esthers pets", "RZfZr1P");
        }

        [Command("Cuties")]
        public async Task Cuties(int CutieNumber)
        {
            var client = new RestClient("https://api.imgur.com/3/album/QMT3epf/images");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Client-ID 3a732d0bb44d8d5");
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            var dataObject = JsonConvert.DeserializeObject<dynamic>(response.Content);

            string link;

            if (CutieNumber <= 0 || CutieNumber > dataObject.data.Count)
            {
                Random r = new Random();
                int randomNumber = r.Next(0, dataObject.data.Count);

                link = dataObject.data[randomNumber].link;
                CutieNumber = randomNumber;
            }
            else
            {
                CutieNumber = CutieNumber - 1;
                link = dataObject.data[CutieNumber].link;
            }

            var CatEmbed = new EmbedBuilder();
            if (dataObject.data[CutieNumber].description.ToString() == "")
            {
                CatEmbed.WithTitle("If you're feeling down just look at this cutie~");
            }
            else
            {
                CatEmbed.WithTitle(dataObject.data[CutieNumber].description.ToString());
            }
            CatEmbed.WithDescription("[Click here to see the full cuties album.](https://imgur.com/a/QMT3epf)");            
            CatEmbed.WithColor(new Color(255, 0, 200));
            CatEmbed.WithImageUrl(link);
            CatEmbed.WithCurrentTimestamp();
            CatEmbed.WithFooter("Hope the cutie helped " + Context.User.Username + " feel better~", Context.User.GetAvatarUrl());
            
            await Context.Channel.SendMessageAsync("", false, CatEmbed.Build());
        }

        [Command("Cuties")]
        public async Task Cuties2()
        {
            await Cuties(0);
        }

        [Command("Cutie")]
        public async Task Cuties3(int CutieNumber)
        {
            await Cuties(CutieNumber);
        }

        [Command("Cutie")]
        public async Task Cuties4()
        {
            await Cuties(0);
        }

        [Command("Cuties Total")]
        public async Task CutiesTotal()
        {
            await GetAlbumLength("cuties", "QMT3epf");
        }

        [Command("Random Person")]
        public async Task RandomPerson(string declaredGender)
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://randomuser.me/api/?gender=" + declaredGender);
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string gender = dataObject.results[0].gender.ToString();
            string firstName = dataObject.results[0].name.first.ToString();
            string lastName = dataObject.results[0].name.last.ToString();
            string picture = dataObject.results[0].picture.large.ToString();
            string birthday = dataObject.results[0].dob.date.ToString();
            string age = dataObject.results[0].dob.age.ToString();
            string phone = dataObject.results[0].cell.ToString();
            string email = dataObject.results[0].email.ToString();
            string password = dataObject.results[0].login.password.ToString();

            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl(picture);
            embed.WithColor(new Color(255, 0, 200));
            embed.WithTitle("Generate Person");
            embed.AddField("First name: ", firstName);
            embed.AddField("Last name: ", lastName);
            embed.AddField("Birthday: ", birthday);
            embed.AddField("Age: ", age);
            embed.AddField("Phone number: ", phone);
            embed.AddField("Email: ", email);
            embed.AddField("Password: ", password);

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("Random Person")]
        public async Task RandomPerson()
        {
            await RandomPerson("");
        }

        [Command("RP")]
        public async Task RandomPerson2(string declaredGender)
        {
            await RandomPerson(declaredGender);
        }

        [Command("RP")]
        public async Task RandomPerson2()
        {
            await RandomPerson("");
        }

        [Command("Cheer")]
        public async Task Cheer([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            var dmChannel = await target.GetOrCreateDMChannelAsync();

            await dmChannel.SendMessageAsync(Utilities.GetFormattedAlert("CHEER"));
        }

        [Command("Piss on")]
        public async Task PissHaha([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("PISS_SELF", Context.User.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("PISS", Context.User.Username, mentionedUser.Username));
                return;
            }

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("PISS", Context.User.Username, message));
        }

        [Command("Piss")]
        public async Task PissHaha2([Remainder]string message = "")
        {
            await PissHaha(message);
        }

        [Command("Pee")]
        public async Task PissHaha3([Remainder]string message = "")
        {
            await PissHaha(message);
        }

        [Command("8ball")]
        public async Task EightBall([Remainder]string message = "")
        {
            string[] options = new string[4] { "8BALL_POSITIVE", "8BALL_DOUBTFUL", "8BALL_UNLIKELY", "8BALL_STOP" };

            Random Index = new Random();
            int num = Index.Next(options.Length);

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert(options[num], message));
        }

        [Command("Russian Roulette")]
        public async Task RussianRoulette()
        {
            SocketUser target = Context.User;
            var account = UserAccountsPlural.GetAccount(target);

            Random Index = new Random();
            int random = Index.Next(bullets);
            bullets = bullets - 1;


            if (random == 0)
            {
                bullets = 6;
                account.Deaths += 1;
                UserAccountsPlural.SaveAccounts();

                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ROULETTE_DIED_NICE", target.Mention));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ROULETTE_DIED", target.Mention));
            }
            else
            {
                if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ROULETTE_SURVIVED_NICE", target.Mention));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("ROULETTE_SURVIVED", target.Mention));
            }

            //await Context.Channel.SendMessageAsync("Retired cuz of how it was being used.");
        }

        [Command("rr")]
        public async Task RussianRoulette2()
        {
            await RussianRoulette();
        }

        [Command("Bullets")]
        public async Task Bulletes()
        {
            SocketUser target = Context.User;

            int bulleteHoles;

            bulleteHoles = bullets - 6;
            bulleteHoles = bulleteHoles * -1;

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BULLETS", bulleteHoles, bullets));
        }

        [Command("Deaths")]
        public async Task Deaths([Remainder]string arg = "")
        {
            bool mentionUser = false;

            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser == null)
            {
                target = Context.User;
            }
            else
            {
                target = mentionedUser;
                mentionUser = true;
            }

            var account = UserAccountsPlural.GetAccount(target);

            if (mentionUser == true)
            {
                if (LikedByYuriBot((SocketGuildUser)target) || UserAccountsPlural.GetAccount(target).isLiked == true)
                {
                    if (account.Deaths == 0)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET_ZERO_NICE", target.Username));
                        return;
                    }
                    else if (account.Deaths == 1)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET_ONE_NICE", target.Username));
                        return;
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET_NICE", target.Username, account.Deaths));
                        return;
                    }
                }

                if (account.Deaths == 0)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET_ZERO", target.Username));
                    return;
                }
                else if (account.Deaths == 1)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET_ONE", target.Username));
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_TARGET", target.Username, account.Deaths));
                    return;
                }
            }

            if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS_NICE", account.Deaths));
                return;
            }

            await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("DEATHS", account.Deaths));
        }

        [Command("Death")]
        public async Task Deaths2([Remainder]string arg = "")
        {
            await Deaths(arg);
        }

        [Command("pfp")]
        public async Task Pfp([Remainder]string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = (mentionedUser == null) ? Context.User : mentionedUser;

            //var pfp = new Image(Context.User.GetAvatarUrl());

            //var converter = new HtmlToImageConverter
            //{
            //    Width = 300,
            //    Height = 320
            //};

            //Convert.ConvertHTML(pfp, )

            var embed = new EmbedBuilder();
            embed.WithColor(new Color(255, 0, 200));
            embed.WithFooter($"Picture requested by {Context.User.Username}", Context.User.GetAvatarUrl());
            embed.WithImageUrl(target.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("Avatar")]
        public async Task Avatar([Remainder]string arg = "")
        {
            await Pfp(arg);
        }

        [Command("Shake")]
        public async Task Shake()
        {
            var ShakeEmbed = new EmbedBuilder();
            ShakeEmbed.WithTitle("UAUAUAUAUAUAUA");
            ShakeEmbed.WithColor(new Color(255, 0, 200));
            ShakeEmbed.WithImageUrl("https://imgur.com/AkrSfsq.gif");
            ShakeEmbed.WithCurrentTimestamp();
            ShakeEmbed.WithFooter("uauaaua perfomed by " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, ShakeEmbed.Build());
        }

        [Command("Shake")]
        public async Task Shake([Remainder]string arg = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            var ShakeEmbed = new EmbedBuilder();
            if (mentionedUser != null)
            {
                ShakeEmbed.WithTitle(mentionedUser.Username + " you have been UAUAUAUAUAUAUA");
            }
            else
            {
                ShakeEmbed.WithTitle(arg + " you have been UAUAUAUAUAUAUA");
            }
            ShakeEmbed.WithColor(new Color(255, 0, 200));
            ShakeEmbed.WithImageUrl("https://imgur.com/AkrSfsq.gif");
            ShakeEmbed.WithCurrentTimestamp();
            ShakeEmbed.WithFooter("uauaaua perfomed by " + Context.User.Username, Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("", false, ShakeEmbed.Build());
        }

        [Command("Squee")]
        public async Task Shake2([Remainder]string arg = "")
        {
            await Shake(arg);
        }

        [Command("Plap")]
        public async Task Plap([Remainder]string arg = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            var PlapEmbed = new EmbedBuilder();
            if (mentionedUser != null)
            {
                PlapEmbed.WithTitle(mentionedUser.Username + " you have been P L A P P E D");
            }
            else
            {
                PlapEmbed.WithTitle(arg + " you have been P L A P P E D");
            }
            PlapEmbed.WithColor(new Color(255, 0, 200));
            PlapEmbed.WithImageUrl("https://imgur.com/9UMZnLD.gif");
            PlapEmbed.WithCurrentTimestamp();
            PlapEmbed.WithFooter("Plap");

            await Context.Channel.SendMessageAsync("", false, PlapEmbed.Build());
        }

        [Command("Kiss")]
        public async Task Kiss([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_KISS_SELF", Context.User.Username));
                        return;
                    }

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("KISS_SELF"));
                    return;

                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("KISS", mentionedUser.Username, Context.User.Username));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("KISS", message, Context.User.Username));
            }
        }

        [Command("Hug")]
        public async Task Hug([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_HUG_SELF", Context.User.Username));
                        return;
                    }

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("HUG_SELF"));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("HUG", mentionedUser.Username, Context.User.Username));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("HUG", message, Context.User.Username));
            }
        }

        [Command("Hold")]
        public async Task Hug2([Remainder]string message = "")
        {
            await Hug(message);
        }

        [Command("Pat")]
        public async Task Pat([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_PAT_SELF", Context.User.Username));
                        return;
                    }

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("PAT_SELF"));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("PAT", mentionedUser.Username, Context.User.Username));
            }
            else
            {
                await Context.Channel.SendFileAsync(Utilities.GetFormattedAlert("PAT", message, Context.User.Username));
            }
        }

        [Command("Pet")]
        public async Task Pat2([Remainder]string message = "")
        {
            await Pat(message);
        }

        [Command("Head Pat")]
        public async Task Pat3([Remainder]string message = "")
        {
            await Pat(message);
        }                

        [Command("Boop")]
        public async Task Boop([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    if (LikedByYuriBot((SocketGuildUser)Context.User) || UserAccountsPlural.GetAccount(Context.User).isLiked == true)
                    {
                        await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("NICE_BOOP_SELF", Context.User.Username));
                        return;
                    }

                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BOOP_SELF"));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BOOP", mentionedUser.Username, Context.User.Username));
            }
            else
            {
                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("BOOP", message, Context.User.Username));
            }
        }

        [Command("Bop")]
        public async Task Boop2([Remainder]string message = "")
        {
            await Boop(message);
        }

        [Command("Sex")]
        public async Task Sex([Remainder]string message = "")
        {
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();

            if (mentionedUser != null)
            {
                if (mentionedUser == Context.User || mentionedUser == null)
                {
                    await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SEX_SELF", Context.User.Username));
                    return;
                }

                await Context.Channel.SendMessageAsync(Utilities.GetFormattedAlert("SEX", Context.User.Username, mentionedUser.Username));
                await Context.Channel.SendMessageAsync("https://imgur.com/l5JwXKV.gif");
                //await Context.Channel.SendFileAsync("https://imgur.com/l5JwXKV.png");
            }
        }

        [Command("Roll")]
        public async Task Roll(int dice)
        {
            dice += 1;

            Random dieRoll = new Random();


            int randomBlue = dieRoll.Next(dice);
            int randomYellow = dieRoll.Next(dice);

            if (randomBlue > randomYellow)
            {
                await Context.Channel.SendMessageAsync("Blue has won with a roll of " + randomBlue + "\nYellow lost with a roll of " + randomYellow);
            }
            else if (randomYellow > randomBlue)
            {
                await Context.Channel.SendMessageAsync("Yellow has won with a roll of " + randomYellow + "\nBlue lost with a roll of " + randomBlue);
            }
            else if (randomBlue == randomYellow)
            {
                await Context.Channel.SendMessageAsync("Yellow and Blue tied with a roll of " + randomYellow);
            }
        }        
    }
}
