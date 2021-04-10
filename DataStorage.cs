using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using DiscordBot.Core.ServerLocationAccount;

namespace DiscordBot
{
    class DataStorage
    {
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        public static void AddPairToStorage(string key, string value)
        {
            pairs.Add(key, value);
            SavedData();
        }

        internal static bool SaveExists(string accountFile)
        {
            throw new NotImplementedException();
        }

        internal static object LoadUserAccounts(string accountFile)
        {
            throw new NotImplementedException();
        }

        public static int GetPairsCount()
        {
            return pairs.Count;
        }

        internal static void SaveUserAccounts(List<UserAccount> accounts, string accountFile)
        {
            throw new NotImplementedException();
        }

        static DataStorage()
        {
            //Load data
            if (!ValidateStorageFile("DataStorage.json")) return;
            string json = File.ReadAllText("DataStroage.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void SavedData()
        {
            //Save the data
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("DataStorage.json", json);
        }

        private static bool ValidateStorageFile(string file)
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "");
                SavedData();
                return false;
            }
            return true;
        }
    }
}
