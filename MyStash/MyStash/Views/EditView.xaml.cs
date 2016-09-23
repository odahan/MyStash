using MyStash.Controls;
using MyStash.Models;
using MyStash.ResX;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class EditView : StashContentPage
    {
        public EditView()
        {
            InitializeComponent();
            Padding = new Thickness(8, Device.OnPlatform(28, 8, 8), 8, 8);
            BindingContext = App.Locator.EditVM;
            var parameter = BindingContext as IParameter;
            parameter?.SetParameter(null); // creation
        }

        public EditView(InfoSheet sheet) : this()
        {
            var parameter = BindingContext as IParameter;
            parameter?.SetParameter(sheet); // edit
            Title = AppResources.ViewEdit_TitleModify;
        }
    }
}
