using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModniteServer.API
{
    public class ApiConfig
    {
        public const string ConfigFile = @"\config.json";

        public const ushort DefaultApiPort = 60101;
        public const ushort DefaultXmppPort = 443;
        public const ushort DefaultMatchmakerPort = 60103;

        static ApiConfig()
        {
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = location + ConfigFile;

            if (!File.Exists(configPath))
            {
                Log.Warning("Config file is missing, so a default config was created. {Path}", configPath);

                string json = JsonConvert.SerializeObject(ApiConfigDefault(), Formatting.Indented);
                File.WriteAllText(configPath, json);
            }

            Current = JsonConvert.DeserializeObject<ApiConfig>(File.ReadAllText(configPath));

            if (Current.AutoCreateAccounts)
                Log.Information("New accounts will be automatically created");

            Log.Information($"Accepting clients on {Current.MinimumVersion.Major}.{Current.MinimumVersion.Minor} or higher {{BuildString}}",
                $"++Fortnite+Release-{Current.MinimumVersion.Major}.{Current.MinimumVersion.Minor}-CL-{Current.MinimumVersion.Build}");
        }

        /// <summary>
        /// Constructs a default config.
        /// </summary>
        static ApiConfig ApiConfigDefault()
        {
            var config = new ApiConfig();
            config.Port = DefaultApiPort;
            config.XmppPort = DefaultXmppPort;
            config.MatchmakerPort = DefaultMatchmakerPort;

            config.AutoCreateAccounts = true;
            config.MinimumVersionString = "6.10.4464155";

            config.DefaultAthenaItems = new HashSet<string>
            {
                "AthenaDance:eid_dancemoves",
                "AthenaGlider:defaultglider",
                "AthenaPickaxe:defaultpickaxe",
                "AthenaCharacter:CID_009_Athena_Commando_M",
                "AthenaCharacter:CID_010_Athena_Commando_M",
                "AthenaCharacter:CID_011_Athena_Commando_M",
                "AthenaCharacter:CID_012_Athena_Commando_M",
                "AthenaCharacter:CID_013_Athena_Commando_F",
                "AthenaCharacter:CID_014_Athena_Commando_F",
                "AthenaCharacter:CID_015_Athena_Commando_F",
                "AthenaCharacter:CID_016_Athena_Commando_F",
            };

            config.EquippedItems = new Dictionary<string, string>
            {
                {"favorite_character",""},
                {"favorite_backpack",""},
                {"favorite_pickaxe",""},
                {"favorite_glider",""},
                {"favorite_skydivecontrail",""},
                {"favorite_dance0",""},
                {"favorite_dance1",""},
                {"favorite_dance2",""},
                {"favorite_dance3",""},
                {"favorite_dance4",""},
                {"favorite_dance5",""},
                {"favorite_musicpack","" },
                {"favorite_loadingscreen",""},
            };

            config.DefaultCoreItems = new HashSet<string>
            {
                "HomebaseBannerColor:defaultcolor1",
                "HomebaseBannerColor:defaultcolor2",
                "HomebaseBannerColor:defaultcolor3",
                "HomebaseBannerColor:defaultcolor4",
                "HomebaseBannerColor:defaultcolor5",
                "HomebaseBannerColor:defaultcolor6",
                "HomebaseBannerColor:defaultcolor7",
                "HomebaseBannerColor:defaultcolor8",
                "HomebaseBannerColor:defaultcolor9",
                "HomebaseBannerColor:defaultcolor10",
                "HomebaseBannerColor:defaultcolor11",
                "HomebaseBannerColor:defaultcolor12",
                "HomebaseBannerColor:defaultcolor13",
                "HomebaseBannerColor:defaultcolor14",
                "HomebaseBannerColor:defaultcolor15",
                "HomebaseBannerColor:defaultcolor16",
                "HomebaseBannerColor:defaultcolor17",
                "HomebaseBannerColor:defaultcolor18",
                "HomebaseBannerColor:defaultcolor19",
                "HomebaseBannerColor:defaultcolor20",
                "HomebaseBannerColor:defaultcolor21",

                "HomebaseBannerIcon:standardbanner1",
                "HomebaseBannerIcon:standardbanner2",
                "HomebaseBannerIcon:standardbanner3",
                "HomebaseBannerIcon:standardbanner4",
                "HomebaseBannerIcon:standardbanner5",
                "HomebaseBannerIcon:standardbanner6",
                "HomebaseBannerIcon:standardbanner7",
                "HomebaseBannerIcon:standardbanner8",
                "HomebaseBannerIcon:standardbanner9",
                "HomebaseBannerIcon:standardbanner10",
                "HomebaseBannerIcon:standardbanner11",
                "HomebaseBannerIcon:standardbanner12",
                "HomebaseBannerIcon:standardbanner13",
                "HomebaseBannerIcon:standardbanner14",
                "HomebaseBannerIcon:standardbanner15",
                "HomebaseBannerIcon:standardbanner16",
                "HomebaseBannerIcon:standardbanner17",
                "HomebaseBannerIcon:standardbanner18",
                "HomebaseBannerIcon:standardbanner19",
                "HomebaseBannerIcon:standardbanner20",
                "HomebaseBannerIcon:standardbanner21",
                "HomebaseBannerIcon:standardbanner22",
                "HomebaseBannerIcon:standardbanner23",
                "HomebaseBannerIcon:standardbanner24",
                "HomebaseBannerIcon:standardbanner25",
                "HomebaseBannerIcon:standardbanner26",
                "HomebaseBannerIcon:standardbanner27",
                "HomebaseBannerIcon:standardbanner28",
                "HomebaseBannerIcon:standardbanner29",
                "HomebaseBannerIcon:standardbanner30",
                "HomebaseBannerIcon:standardbanner31"
            };

            config.DailyShopItems = new HashSet<string>
            {
                "AthenaCharacter:CID_003_Athena_Commando_F_Default",
                "AthenaCharacter:CID_004_Athena_Commando_F_Default",
                "AthenaCharacter:CID_005_Athena_Commando_M_Default",
                "AthenaCharacter:CID_006_Athena_Commando_M_Default",
                "AthenaCharacter:CID_007_Athena_Commando_M_Default",
                "AthenaCharacter:CID_008_Athena_Commando_M_Default"
            };

            config.FeaturedShopItems = new HashSet<string>
            {
                "AthenaCharacter:CID_002_Athena_Commando_F_Default",
                "AthenaCharacter:CID_001_Athena_Commando_F_Default"
            };
#if DEBUG
            config.LogHttpRequests = true;
            config.Log404 = true;
#endif
            config.Season = 6;

            config.ClientEvents = new List<string>();
            return config;
        }

        public static ApiConfig Current { get; }

        [JsonIgnore]
        public ILogger Logger
        {
            get { return Log.Logger; }
            set { Log.Logger = value; }
        }

        [JsonProperty(PropertyName = "MinimumVersion")]
        public string MinimumVersionString { get; set; }

        /// <summary>
        /// Gets the minimum version clients have to be on in order to connect to this server.
        /// </summary>
        [JsonIgnore]
        public Version MinimumVersion { get { return new Version(MinimumVersionString); } }

        /// <summary>
        /// Gets or sets whether the server will automatically create a new account whenever a user
        /// attempts to log in with an account that does not exist.
        /// </summary>
        public bool AutoCreateAccounts { get; set; }

        /// <summary>
        /// Gets or sets the port for the API server.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Gets or sets the port for the XMPP server.
        /// </summary>
        public ushort XmppPort { get; set; }

        /// <summary>
        /// Gets or sets the port for the matchmaker.
        /// </summary>
        public ushort MatchmakerPort { get; set; }

        /// <summary>
        /// Gets or sets the list of cosmetics, quests, and other items to give to new accounts.
        /// </summary>
        public HashSet<string> DefaultAthenaItems { get; set; }

        /// <summary>
        /// Gets or sets the list of banners and colors to give to new accounts.
        /// </summary>
        public HashSet<string> DefaultCoreItems { get; set; }

        /// <summary>
        /// Gets or sets the list of currently equipped items that will be on an account.
        /// </summary>
        public Dictionary<string, string> EquippedItems { get; set; }

        /// <summary>
        /// Gets or sets whether to log all valid HTTP requests.
        /// </summary>
        public bool LogHttpRequests { get; set; }

        /// <summary>
        /// Gets or sets the daily item shop items.
        /// </summary>
        public HashSet<string> DailyShopItems { get; set; }

        /// <summary>
        /// Gets or sets the featured shop items
        ///</summary>
        public HashSet<string> FeaturedShopItems { get; set; }

        /// <summary>
        /// Gets or sets whether to log all HTTP requests to nonexistent endpoints. This setting is
        /// independent from <see cref="LogHttpRequests"/>.
        /// </summary>
        public bool Log404 { get; set; }

        /// <summary>
        /// Gets or sets the list of events.
        /// </summary>
        public List<string> ClientEvents { get; set; }

        /// <summary>
        /// Gets or sets the current season (clients will need to restart)
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// Sets the default season level for an account
        /// </summary>
        public int DefaultLevel { get; set; }

        /// <summary>
        /// Sets the default total level for an account
        /// </summary>
        public int DefaultTotalLevel { get; set; }
    }
}