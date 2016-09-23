using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MyStash.Crouton;
using MyStash.ViewModels;
using MyStash.Views;
using Xamarin.Forms;

namespace MyStash.Service
{


    public class GlobalLocator
    {
        static GlobalLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            var navigation = new NavigationService();
            SimpleIoc.Default.Register<INavigationService2>(() => navigation);
            SimpleIoc.Default.Register<IDataProvider>(() => new DataProvider());
            SimpleIoc.Default.Register<IAppSettings>(() => new AppSettings());
            // ReSharper disable once AssignNullToNotNullAttribute
            SimpleIoc.Default.Register(() => Application.Current as ILoginSwitch);


            initializeNavigation(navigation);
        }

        private static void initializeNavigation(NavigationService navigationService)
        {
            // key => View pour la navigation
            navigationService.Configure(PageName.MainPage.ToString(), typeof(MainView));
            navigationService.Configure(PageName.GeneratePw.ToString(), typeof(GeneratePwView));
            navigationService.Configure(PageName.SetPwPage.ToString(), typeof(SetPwView));
            navigationService.Configure(PageName.MainListPage.ToString(), typeof(MainListView));
            navigationService.Configure(PageName.CheckPw.ToString(), typeof(CheckPwView));
            navigationService.Configure(PageName.DetailPage.ToString(), typeof(DetailView));
            navigationService.Configure(PageName.EditPage.ToString(), typeof(EditView));
            navigationService.Configure(PageName.SettingsPage.ToString(), typeof(SettingsView));
            navigationService.Configure(PageName.DataImportPage.ToString(), typeof(ImportView));




            // enregistrement des types de ViewModels pour le locator
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GeneratePwViewModel>();
            SimpleIoc.Default.Register<SetPwViewModel>();
            SimpleIoc.Default.Register<MainListViewModel>();
            SimpleIoc.Default.Register<CheckPwViewModel>();
            SimpleIoc.Default.Register<DetailViewModel>();
            SimpleIoc.Default.Register<EditViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ImportViewModel>();



        }

        // not all VM need to be cached. To manage correctly a fresh new state each time it is better to recreate an instance in most cases.

        //cached VM's
        public object MainVM => ServiceLocator.Current.GetInstance<MainViewModel>();

        public object MainListVM => ServiceLocator.Current.GetInstance<MainListViewModel>();

        // non cached VM's
        public object GenPwVM => new GeneratePwViewModel(); 

        public object SetPwVM => new SetPwViewModel(); 

        public object PwCheckVM => new CheckPwViewModel(); 

        public object DetailVM => new DetailViewModel(); 

        // need a different one each time, easier to manage
        public object EditVM => new EditViewModel(); 

        public object SettingsVM => new SettingsViewModel(); 

        public object ImportDataVM => new ImportViewModel();

        // navigation service
        // original interface from Mvvm Light
        public INavigationService NavigationService => ServiceLocator.Current.GetInstance<INavigationService>();
        // enhanced version by OD with modal support
        public INavigationService2 NavigationService2 => ServiceLocator.Current.GetInstance<INavigationService2>();


        // dialog service
        public IDialogService DialogService => ServiceLocator.Current.GetInstance<IDialogService>();

        // Data service
        public IDataProvider DataService => ServiceLocator.Current.GetInstance<IDataProvider>();

        // Settings service
        public IAppSettings AppSettings => ServiceLocator.Current.GetInstance<IAppSettings>();

        // Login service
        public ILoginSwitch LoginSwitch => ServiceLocator.Current.GetInstance<ILoginSwitch>();

        // Toast
        public IToastNotificator Toasts => DependencyService.Get<IToastNotificator>();
    }
}