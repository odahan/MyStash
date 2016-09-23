using JetBrains.Annotations;
using MyStash.Controls;
using MyStash.Models;
using MyStash.ResX;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class DetailView : StashContentPage
    {
        [LocalizationRequired(false)]
        public DetailView()
        {
            InitializeComponent();
            Padding = new Thickness(8, Device.OnPlatform(28, 8, 8), 8, 8);
            BindingContext = App.Locator.DetailVM;
            chkPwItem = new ToolbarItem
            {
                Text = AppResources.ViewDetail_CheckPw,
                Icon = "toolbarcheckpw.png",
            };
            chkPwItem.SetBinding(MenuItem.CommandProperty, new Binding("CheckPasswordCommand"));

            moreTimeItem = new ToolbarItem
            {
                Text = AppResources.DetailViewModel_DetailViewModel_More_time,
                Icon = "toolbartimer.png",
            };
            moreTimeItem.SetBinding(MenuItem.CommandProperty, new Binding("MoreTimeCommand"));

            editItem = new ToolbarItem
            {
                Text = AppResources.DetailViewModel_DetailViewModel_Edit,
                Icon = "edit.png",
            };
            editItem.SetBinding(MenuItem.CommandProperty, new Binding("EditCommand"));
        }

        private readonly ToolbarItem chkPwItem;
        private readonly ToolbarItem moreTimeItem;
        private readonly ToolbarItem editItem;

        public DetailView(InfoSheet infoSheet) : this()
        {
            (BindingContext as IParameter)?.SetParameter(infoSheet);
            ToolbarItems.Add(moreTimeItem);
            if (!string.IsNullOrWhiteSpace(infoSheet?.Password) && infoSheet.Category == InfoSheet.CategoryFilter.Login)
                ToolbarItems.Add(chkPwItem);
            ToolbarItems.Add(editItem);
        }
    }
}
