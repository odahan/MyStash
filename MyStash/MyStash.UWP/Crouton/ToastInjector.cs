using System;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyStash.UWP.Crouton
{
    internal static class ToastInjector
    {
        private static bool injected = false;

        // TODO: I am using an async void, bad programmer
        public static async void Inject()
        {
            if (injected)
                return;

            injected = true;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // Let's inject our toast into frame using a special Frame template defined in FrameStyle.xaml
                var frameStyleRd = new ResourceDictionary();
                var frame = (Frame)Window.Current.Content;
                frameStyleRd.Source = new Uri("ms-appx:///Crouton/FrameStyle.xaml",UriKind.RelativeOrAbsolute);
                Application.Current.Resources.MergedDictionaries.Add(frameStyleRd);
                frame.Style = Application.Current.Resources["MainFrameStyle"] as Style;
               
            });
        }
    }
}
