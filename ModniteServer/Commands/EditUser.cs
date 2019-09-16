using ModniteServer.API;
using ModniteServer.API.Accounts;
using ModniteServer.Views;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

// TO DO CLEANUP:
// Array stupidity
// OptionInfo stuff
//
namespace ModniteServer.Commands
{
    public class OptionInfo
    {
        public System.Windows.Controls.TextBox windowspace;
        public string accountString;
        public string stringName;
        public bool favorite;
        public OptionInfo(System.Windows.Controls.TextBox windowspacee, string accountStringe, string stringNamee, bool fav)
        {
            windowspace = windowspacee;
            accountString = accountStringe;
            stringName = stringNamee;
            favorite = fav;
        }
    }
    /// <summary>
    /// User command for editing an account.
    /// </summary>
    /// 
    public sealed class EditUserCommand : IUserCommand
    {
        public string Description => "Opens a window that allows you to edit the user.";
        public string ExampleArgs => "player123";
        public string Args => "<userid>";

        public void Handle(string[] args)
        {
            if (args.Length == 1)
            {
                if (AccountManager.AccountExists(args[0]))
                {
                    var account = AccountManager.GetAccount(args[0]); // Awful code, Tried loops, didn't work :(
                    EditUserWindow window = new EditUserWindow();

                    var tings = new List<OptionInfo>() { // array of all the options (excluding athena/core items)
                        new OptionInfo(window.Account, account.AccountId,"",false),
                        new OptionInfo(window.Email,account.Email,"",false),
                        new OptionInfo(window.DisplayName, account.DisplayName,"",false),
                        new OptionInfo(window.Country, account.Country,"",false),
                        new OptionInfo(window.Language, account.PreferredLanguage,"",false),
                        new OptionInfo(window.Level, Convert.ToString(account.Level),"",false),
                        new OptionInfo(window.XP, Convert.ToString(account.XP),"",false),
                        new OptionInfo(window.Dance0, account.EquippedItems["favorite_dance0"],"favorite_dance0",true),
                        new OptionInfo(window.Dance1, account.EquippedItems["favorite_dance1"],"favorite_dance1",true),
                        new OptionInfo(window.Dance2, account.EquippedItems["favorite_dance2"],"favorite_dance2",true),
                        new OptionInfo(window.Dance3, account.EquippedItems["favorite_dance3"],"favorite_dance3",true),
                        new OptionInfo(window.Dance4, account.EquippedItems["favorite_dance4"],"favorite_dance4",true),
                        new OptionInfo(window.Dance5, account.EquippedItems["favorite_dance5"],"favorite_dance5",true),
                        new OptionInfo(window.Character, account.EquippedItems["favorite_character"],"favorite_character",true),
                        new OptionInfo(window.Backpack, account.EquippedItems["favorite_backpack"],"favorite_backpack",true),
                        new OptionInfo(window.Pickaxe, account.EquippedItems["favorite_pickaxe"],"favorite_pickaxe",true),
                        new OptionInfo(window.Glider, account.EquippedItems["favorite_glider"],"favorite_glider",true),
                        new OptionInfo(window.Contrail, account.EquippedItems["favorite_skydivecontrail"],"favorite_skydivecontrail",true),
                        new OptionInfo(window.Music, account.EquippedItems["favorite_musicpack"],"favorite_musicpack",true),
                        new OptionInfo(window.LoadingScreen, account.EquippedItems["favorite_loadingscreen"],"favorite_loadingscreen",true),
                        new OptionInfo(window.Season, Convert.ToString(ApiConfig.Current.Season),"",false),
                        new OptionInfo(window.BattlePassLevel, Convert.ToString(account.PassLevel),"",false),
                        new OptionInfo(window.Stars,Convert.ToString(account.PassXP),"",false),
                        new OptionInfo(window.AccountLevel,Convert.ToString(account.TotalLevel),"",false)
                    };
                    foreach (var i in tings)
                    {
                        i.windowspace.Text = i.accountString;
                    }
                    window.hasBattlePass.IsChecked = account.BattlePass;
                    window.ban.IsChecked = account.IsBanned;

                    string athenaItems = "";
                    foreach (string item in account.AthenaItems.ToArray())
                    {
                        athenaItems = athenaItems + item + "\n";
                    }
                    window.athenaItems.Text = athenaItems;


                    string coreItems = "";
                    foreach (string item in account.CoreItems.ToArray())
                    {
                        coreItems = coreItems + item + "\n";
                    }
                    window.coreItems.Text = coreItems;


                    window.ShowDialog();

                    int level = 0;
                    int xp = 0;
                    int season = 0;
                    int passxp = 0;
                    int passlevel = 0;
                    int totalevel = 0;
                    try
                    {
                        level = Convert.ToInt32(window.Level.Text);
                        xp = Convert.ToInt32(window.XP.Text);
                        season = Convert.ToInt32(window.Season.Text);
                        passxp = Convert.ToInt32(window.Stars.Text);
                        passlevel = Convert.ToInt32(window.BattlePassLevel.Text);
                        totalevel = Convert.ToInt32(window.AccountLevel.Text);
                    }
                    catch
                    {
                        MessageBox.Show("XP, Level, season and battle pass level/xp must all be integers! Any changes you made were not saved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    account.AccountId = window.Account.Text; // Can't clean up this :(
                    account.Email = window.Email.Text;
                    account.DisplayName = window.DisplayName.Text;
                    account.Country = window.Country.Text;
                    account.PreferredLanguage = window.Language.Text;
                    account.Level = level;
                    account.XP = xp;
                    ApiConfig.Current.Season = season;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();
                    account.PassLevel = passlevel;
                    account.PassXP = passxp;
                    account.TotalLevel = totalevel;

                    var athenaafter = window.athenaItems.Text.Replace(",", "").Replace(" ", "").Replace("\"", "").Replace("\r", "").Replace("   ", "").Split('\n');
                    var coreafter = window.coreItems.Text.Replace(",", "").Replace(" ", "").Replace("\"", "").Replace("\r", "").Replace("   ", "").Split('\n');
                    if (athenaafter[athenaafter.Length - 1] == "")
                        account.AthenaItems = athenaafter.Take(athenaafter.Length - 1).ToHashSet();
                    else
                        account.AthenaItems = athenaafter.ToHashSet();

                    if (coreafter[coreafter.Length - 1] == "")
                        account.CoreItems = coreafter.Take(coreafter.Length - 1).ToHashSet();
                    else
                        account.CoreItems = coreafter.ToHashSet();



                    // cleanup - favorite_character etc should be in class, and indexed!!
                    foreach (var i in tings)
                    {
                        if (i.favorite == true)
                        {
                            account.EquippedItems[i.stringName] = i.windowspace.Text;
                        }
                    }
                    account.BattlePass = window.hasBattlePass.IsChecked ?? false;
                    account.IsBanned = window.ban.IsChecked ?? false;
                    AccountManager.SaveAccounts();
                }
                else
                {
                    Log.Error($"Account '{args[0]}' does not exist");
                }
            }
            else
            {
                Log.Error("Invalid arguments");
            }
        }
    }
}