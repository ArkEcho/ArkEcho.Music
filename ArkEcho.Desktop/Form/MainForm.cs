using ArkEcho.Core;
using ArkEcho.RazorPage;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArkEcho.Desktop
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer timerSplash = null;
        private SynchronizationContext context = null;
        private Rest rest = null;
        private Logger logger = null;

        private const string desktopConfigFileName = "ArkEchoDesktopConfig.json";

        public RestLoggingWorker LoggingWorker { get; private set; } = null;
        public RazorConfig Config { get; private set; } = null;


        public MainForm()
        {
            InitializeComponent();
        }

        private void TimerSplash_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO: Abhängig von Config laden, ohne Timer!
            context?.Send(o => pictureSplash.Hide(), null);
            context?.Send(o => blazorWebView.Show(), null);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            blazorWebView.Hide();

            context = SynchronizationContext.Current;
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            await Init();
        }

        private async Task Init()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Config = new RazorConfig(desktopConfigFileName);

            bool success = await Config.LoadFromFile(executingLocation, true);
            if (!success)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Desktop");
                MessageBox.Show("No Config File found/Error Loading -> created new one, please configure!", "ArkEcho", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }

            rest = new Rest(Config.ServerAddress, Config.Compression);
            if (!rest.CheckConnection())
            {
                Console.WriteLine("### No Response from Server! Maybe its Offline! Stopping Desktop");
                MessageBox.Show("No Response from Server! Maybe its Offline!", "ArkEcho", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }

            LoggingWorker = new RestLoggingWorker(rest, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger(Resources.ARKECHODESKTOP, "Form", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Desktop:");
            string config = await Config.SaveToJsonString();
            logger.LogStatic($"\r\n{config}");

            var services = new ServiceCollection();

            services.AddWindowsFormsBlazorWebView();

            services.AddSingleton(LoggingWorker);
            services.AddScoped<ILocalStorage, DesktopLocalStorage>();
            services.AddScoped<IAppModel, DesktopAppModel>();

            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = services.BuildServiceProvider();
            blazorWebView.RootComponents.Add<BlazorApp>("#app");

            timerSplash = new System.Timers.Timer();
            timerSplash.Interval = 1000;
            timerSplash.Elapsed += TimerSplash_Elapsed;
            timerSplash.Start();
        }
    }
}