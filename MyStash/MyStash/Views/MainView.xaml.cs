using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using MyStash.Controls;
using MyStash.Helpers;
using Xamarin.Forms;


namespace MyStash.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage<string>>(this, message =>
                                                                          { 
                                                                              if (message.Notification != Utils.GlobalMessages.AnimateDoorButton.ToString()) return;
                                                                              var obj = VisualTreeHelper.FindObject(this, o =>
                                                                                       {
                                                                                           var l = o as Label;
                                                                                           if (l?.ClassId == null) return false;
                                                                                           if (!l.ClassId.StartsWith("*")) return false;
                                                                                           return l.Text == message.Content;
                                                                                       });
                                                                              if (obj == null) return;
                                                                              var label = (AnimatedTapLabel)obj;
                                                                              var cid = label.ClassId;
                                                                              cid = cid.Replace('*', '+');
                                                                              obj = VisualTreeHelper.FindObject(this, o =>
                                                                                        {
                                                                                            var c = o as TapImage;
                                                                                            if (c == null) return false;
                                                                                            return c.ClassId == cid;
                                                                                        });
                                                                              if (obj == null) return;
                                                                              var tc = (TapImage)obj;
                                                                              tc.PlayAnimation();
                                                                              label.PlayAnimation();
                                                                          });
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0); // iOS
            initNumbers();
            BindingContext = App.Locator.MainVM;
        }

        private void initNumbers()
        {
            const string digits = "0123456789ABCDEF";
            var numbers = App.Locator.AppSettings.AreDigitsShuffled ? digits.Shuffle() : digits.ToCharArray().ToList();
            var labels = VisualTreeHelper.FindObjects(this, o =>
                                                            {
                                                                var v = (o as VisualElement);
                                                                return !string.IsNullOrWhiteSpace(v?.ClassId) && v.ClassId.StartsWith("*");
                                                            });
            var cpt = 0;
            foreach (var o in labels)
            {
                var label = (AnimatedTapLabel) o;
                if (label != null) label.Text = numbers[cpt++] + "";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            initNumbers();
            Opacity = 0d;
#pragma warning disable 4014
            this.FadeTo(1, 200).ContinueWith(task => Device.BeginInvokeOnMainThread(() => Opacity = 1));
#pragma warning restore 4014
        }
    }
}
