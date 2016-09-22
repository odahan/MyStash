using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class SetPwView : ContentPage
    {
        public SetPwView(bool changePassWord)
        {
            InitializeComponent();
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0); // iOS
            BindingContext = App.Locator.SetPwVM;
            CancelButton.IsVisible = changePassWord;
            OkButton.HorizontalOptions = !changePassWord ? LayoutOptions.Center : LayoutOptions.End;
            var parameter = BindingContext as IParameter;
            parameter?.SetParameter(changePassWord);
        }
        
    }
}
