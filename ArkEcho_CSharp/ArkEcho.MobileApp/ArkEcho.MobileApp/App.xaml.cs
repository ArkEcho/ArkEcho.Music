using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ArkEcho.MobileApp.Services;
using ArkEcho.MobileApp.Views;

namespace ArkEcho.MobileApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
