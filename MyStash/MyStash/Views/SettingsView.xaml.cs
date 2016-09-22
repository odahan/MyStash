using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class SettingsView : ContentPage
    {
        public SettingsView()
        {
            InitializeComponent();
            Padding = new Thickness(8, Device.OnPlatform(20, 0, 0), 8, 8); // iOS
            BindingContext = App.Locator.SettingsVM;
        }
    }
}
