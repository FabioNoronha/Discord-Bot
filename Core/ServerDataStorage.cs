using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using DiscordBot.Core.ServerLocationAccount;

namespace DiscordBot.Core
{
    public static class ServerDataStorage
    {
        //Save all user accounts
        public static void SaveServer(IEnumerable<Greetings> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }



        //Get all user accounts
        public static IEnumerable<Greetings> LoadServer(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Greetings>>(json);
        }



        public static bool ServerExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
