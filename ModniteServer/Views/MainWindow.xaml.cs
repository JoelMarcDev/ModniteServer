using ModniteServer.API;
using ModniteServer.API.Accounts;
using ModniteServer.Controls;
using ModniteServer.ViewModels;
using Newtonsoft.Json;
using Serilog;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace ModniteServer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.Logger = new LoggerConfiguration().WriteTo.Sink(new LogStreamSink(this.logStream)).CreateLogger();
            DataContext = ViewModel = new MainViewModel();

            this.commandTextBox.GotFocus += delegate { this.commandTextBox.Tag = ""; };
            this.commandTextBox.LostFocus += delegate { this.commandTextBox.Tag = "Enter command"; };
            this.commandTextBox.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    ViewModel.InvokeCommand();
            };

            this.Closing += OnMainWindowClosing;
        }

        public MainViewModel ViewModel { get; }

        void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            AccountManager.SaveAccounts();
            
            // Save any config changes
            string json = JsonConvert.SerializeObject(ApiConfig.Current, Formatting.Indented);
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configPath = location + ApiConfig.ConfigFile;
            File.WriteAllText(configPath, json);
        }
    }
}