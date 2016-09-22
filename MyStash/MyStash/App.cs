using System;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MyStash.Helpers;
using MyStash.Service;
using MyStash.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyStash
{
    public class App : Application, ILoginSwitch
    {
        public App()
        {
            Locator = new GlobalLocator();
            if (Locator.AppSettings.PreferredCulture != SupportedCulture.Default)
                Translator.SetLocale(Locator.AppSettings.CultureCodeDictionary[Locator.AppSettings.PreferredCulture]);
            //Translator.SetLocale( Locator.AppSettings.CultureCodeDictionary[SupportedCulture.German]);

            MainPage = new NavigationPage(new ContentPage());
            if (string.IsNullOrWhiteSpace(ServiceLocator.Current.GetInstance<IAppSettings>().GetDoorLock()))
                CreatePassword();
            else LogOut();
           //MainPage = new NavigationPage(new SettingsView());
        }

        public static GlobalLocator Locator { get; private set; }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.OnPropertyChanged(propertyName);
            if (propertyName != nameof(MainPage)) return;
            var dialog = ServiceLocator.Current.GetInstance<IDialogService>() as DialogService;
            dialog?.Initialize(MainPage);
            var nav = Locator.NavigationService2;
            nav.Initialize(MainPage);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            timeoutTimer?.Stop();
            timeoutTimer = null;
            dimmedScreen();
        }

        protected override void OnResume()
        {
            if (Locator.AppSettings.LockOnHide) LogOut();
        }

        public void ShowMainPage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (Locator.AppSettings.AutoTimeOut) launchTimeout();
                replaceRoot(new MainListView());
                NavigationPage.SetHasNavigationBar(MainPage, true);
            });
        }

        public void LogOut()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                timeoutTimer?.Stop();
                timeoutTimer = null;
                if (MainPage is MainView || (MainPage as NavigationPage)?.CurrentPage is MainView) return;
                replaceRoot(new MainView());
                NavigationPage.SetHasNavigationBar(MainPage, false);
            });
        }

        public void CreatePassword()
        {
            replaceRoot(new SetPwView(false));
            NavigationPage.SetHasNavigationBar(MainPage, false);
        }

        // ReSharper disable once AssignNullToNotNullAttribute
        private NavigationPage currentNavigationPage => MainPage as NavigationPage;

        private void replaceRoot(Page page)
        {
            Device.BeginInvokeOnMainThread(() =>
                                           {
                                               var root = currentNavigationPage.Navigation.NavigationStack[0];
                                               currentNavigationPage.Navigation.InsertPageBefore(page, root);
                                               popToRoot();
                                           });
        }

        private void popToRoot()
        {
            Device.BeginInvokeOnMainThread(async () =>
                                           {
                                               await currentNavigationPage.PopToRootAsync(false);
                                               /*var existingPages = currentNavigationPage.Navigation.NavigationStack.ToList();
                                               for (var i = 1; i < existingPages.Count; i++)
                                               {
                                                   currentNavigationPage.Navigation.RemovePage(existingPages[i]);
                                               }*/
                                           });
        }


        private ITimer timeoutTimer;
        private void launchTimeout()
        {
            if (Locator.AppSettings.TimeOutSeconds < 1) return;
            timeoutTimer = DependencyService.Get<ITimer>();
            timeoutTimer.IntervalTime = TimeSpan.FromSeconds(Locator.AppSettings.TimeOutSeconds);
            timeoutTimer.AutoReset = false;
            timeoutTimer.Elapsed += (sender, args) =>
                                    {
                                        timeoutTimer?.Stop();
                                        timeoutTimer = null;
                                        LogOut();
                                    };
        }

        public void ResetTimeout()
        {
            timeoutTimer?.Reset();
        }

        private void dimmedScreen()
        {
            if (MainPage == null) return;
            if (!Locator.AppSettings.AutoTimeOut) return;
            Page v = MainPage as MainView;
            if (v != null) return;
            NavigationPage np = MainPage as NavigationPage;
            if (np == null) return;
            if (np.CurrentPage is MainView) return;
            np.CurrentPage.Opacity = 0;


        }

    }
}
