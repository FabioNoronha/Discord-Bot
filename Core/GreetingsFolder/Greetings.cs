using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.ServerLocationAccount
{
    public class Greetings
    {
        public ulong ServerID { get; set; }

        public bool GreetingTurned;

        public string GreetingMessage { get; set; }

        public ulong GreetingChannelID { get; set; }

        public bool GoodbyeTurned;

        public string GoodbyeMessage { get; set; }

        public ulong GoodbyeChannelID { get; set; }
    }
}
