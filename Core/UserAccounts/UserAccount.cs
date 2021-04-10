using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.ServerLocationAccount
{
    public class UserAccount
    {
        public ulong ID { get; set; }

        public uint Points { get; set; }

        public uint XP { get; set; }

        public uint Deaths { get; set; }

        public bool isLiked;
    }
}
