using System;
using MyStash.Controls;
using MyStash.Helpers;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class GeneratePwView : StashContentPage
    {
        public GeneratePwView()
        {
            InitializeComponent();
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0); // iOS
            BindingContext = App.Locator.GenPwVM;
        }

        public GeneratePwView(bool modalMode) : this()
        {
            ModalBackButton.IsVisible = modalMode;
        }

        private void TapGestureRecognizerOnTapped(object sender, EventArgs e)
        {
            var genericCommand = BindingContext as IGenericCommand;
            genericCommand?.SendCommand(Utils.GlobalMessages.CopyToClipbard.ToString());
            CopyButton.FadeTo(0, 200, Easing.CubicOut).ContinueWith(t => Device.BeginInvokeOnMainThread(() => CopyButton.Opacity = 1)).ConfigureAwait(false);
            CopyButton.ScaleTo(1.5, 200, Easing.CubicOut).ContinueWith(t => Device.BeginInvokeOnMainThread(() => CopyButton.Scale = 1)).ConfigureAwait(false);
        }
    }
}
