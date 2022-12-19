using ArkEcho.Core;
using ArkEcho.RazorPage;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace ArkEcho.Desktop
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer timerSplash = null;

        private SynchronizationContext context = null;

        // TODO: Desktop Logging
        // TODO: Desktop Config

        public MainForm()
        {
            InitializeComponent();

            context = SynchronizationContext.Current;

            var services = new ServiceCollection();

            services.AddWindowsFormsBlazorWebView();

            services.AddScoped<ILocalStorage, DesktopLocalStorage>();
            services.AddScoped<IAppModel, DesktopAppModel>();

            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = services.BuildServiceProvider();
            blazorWebView.RootComponents.Add<App>("#app");

            timerSplash = new System.Timers.Timer();
            timerSplash.Interval = 2000;
            timerSplash.Elapsed += TimerSplash_Elapsed;
            timerSplash.Start();
        }

        private void TimerSplash_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // TODO: Abhängig von Config laden, ohne Timer!
            context?.Send(o => pictureSplash.Hide(), null);
            context?.Send(o => blazorWebView.Show(), null);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            blazorWebView.Hide();
        }
    }
}