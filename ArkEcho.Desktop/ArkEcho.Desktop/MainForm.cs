using ArkEcho.Core;
using ArkEcho.Desktop.Data;
using ArkEcho.RazorPage;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace ArkEcho.Desktop
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var services = new ServiceCollection();

            services.AddWindowsFormsBlazorWebView();

            services.AddScoped<ILocalStorage, DesktopLocalStorage>();
            services.AddScoped<IAppModel, DesktopAppModel>();

            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = services.BuildServiceProvider();
            blazorWebView.RootComponents.Add<App>("#app");
        }
    }
}