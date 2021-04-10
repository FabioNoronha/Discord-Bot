using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Core.ServerLocationAccount
{
    public static class GreetingsPlural
    {
        private static List<Greetings> servers;

        private static string accountFile = "Resources/server.json";

        static GreetingsPlural()
        {
            if (ServerDataStorage.ServerExists(accountFile))
            {
                servers = ServerDataStorage.LoadServer(accountFile).ToList();
            }
            else
            {
                servers = new List<Greetings>();
                SaveServer();
            }
        }

        public static void SaveServer()
        {
            ServerDataStorage.SaveServer(servers, accountFile);
        }

        public static Greetings GetServer(SocketGuild server)
        {
            var channel = server.DefaultChannel.Id;
            return GetOrCreateGreeting(server.Id, channel);
        }

        private static Greetings GetOrCreateGreeting(ulong ID, ulong channelID)
        {
            var result = from a in servers where a.ServerID == ID select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateServerGreeting(ID, channelID);

            return account;
        }

        private static Greetings CreateServerGreeting(ulong ID, ulong channelID)
        {
            var newAccount = new Greetings()
            {
                ServerID = ID,
                GreetingTurned = false,
                GreetingMessage = "",
                GreetingChannelID = channelID,
                GoodbyeTurned = false,
                GoodbyeMessage = "",
                GoodbyeChannelID = channelID
            };

            servers.Add(newAccount);
            SaveServer();
            return newAccount;
        }
    }
}
