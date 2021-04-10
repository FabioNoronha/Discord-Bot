using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot.Core.ServerLocationAccount
{
    public static class UserAccountsPlural
    {
        private static List<UserAccount> accounts;

        private static string accountFile = "Resources/accounts.json";       

        static UserAccountsPlural()
        {
            if (UserDataStorage.SaveExists(accountFile))
            {
                accounts = UserDataStorage.LoadUserAccounts(accountFile).ToList();
            } else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts()
        {
            UserDataStorage.SaveUserAccounts(accounts, accountFile);
        }

        public static UserAccount GetAccount(SocketUser user)
        {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id)
        {
            var result = from a in accounts where a.ID == id select a;

            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount()
            {
                ID = id,
                Points = 0,
                XP = 0,
                Deaths = 0,
                isLiked = false
            };

            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
