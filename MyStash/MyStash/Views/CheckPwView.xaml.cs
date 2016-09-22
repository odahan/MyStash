using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class CheckPwView : ContentPage
    {
        public CheckPwView()
        {
            InitializeComponent();
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0); // iOS
            BindingContext = App.Locator.PwCheckVM;
        }

        public CheckPwView(string password) : this()
        {
            (BindingContext as IParameter)?.SetParameter(password);
        }

        private void ListViewOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView) sender).SelectedItem = null;
        }
    }
}
