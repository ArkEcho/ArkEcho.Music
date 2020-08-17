using System.ComponentModel;
using Xamarin.Forms;
using ArkEcho.MobileApp.ViewModels;

namespace ArkEcho.MobileApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}