using MyStash.Controls;
using MyStash.Helpers;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class MainListView : StashTabbedPage
    {
        public MainListView()
        {
            InitializeComponent();
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0); // iOS
            BindingContext = App.Locator.MainListVM;
        }

        private void listviewItempTapped(object sender, ItemTappedEventArgs e)
        {
            App.Locator.LoginSwitch.ResetTimeout();
            var genericCommand = BindingContext as IGenericCommand;
            genericCommand?.SendCommand(Utils.GlobalCommands.ListviewTapped.ToString(),e.Item);
            var lv = sender as ListView;
            if (lv != null) lv.SelectedItem = null;
        }

        private void itemInOut(object sender, ItemVisibilityEventArgs e)
        {
            App.Locator.LoginSwitch.ResetTimeout();
        }
    }
}
