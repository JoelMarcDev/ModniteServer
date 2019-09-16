using System;
using System.Collections.Generic;

namespace ModniteServer.API.Accounts
{
    public class Account
    {
        public Account()
        {
            Country = "US";
            PreferredLanguage = "en";
            DisplayNameHistory = new List<string>();
        }

        public string AccountId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string DisplayName { get; set; }

        public List<string> DisplayNameHistory { get; set; }

        public int FailedLoginAttempts { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime LastLogin { get; set; }

        public string Country { get; set; }

        public string PreferredLanguage { get; set; }

        public bool IsBanned { get; set; }

        public HashSet<string> AthenaItems { get; set; }

        public HashSet<string> CoreItems { get; set; }

        public Dictionary<string, string> EquippedItems { get; set; }

        // Battle pass
        public int PassLevel { get; set; }

        public int PassXP { get; set; }

        public bool BattlePass { get; set; }
		
        // Implement levels
        public int Level { get; set; }
		
        public int TotalLevel { get; set; }
		
        public int XP { get; set; }
    }
}
