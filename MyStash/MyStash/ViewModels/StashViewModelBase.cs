using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using MyStash.Crouton;
using MyStash.Service;

namespace MyStash.ViewModels
{
    public class StashViewModelBase : ViewModelBase, IParameter, IGenericCommand
    {
        #region shortcuts to services

        public GlobalLocator Locator => App.Locator;
        public IDialogService DialogService => App.Locator.DialogService;
        public INavigationService2 Navigation => App.Locator.NavigationService2;
        public IAppSettings Settings => App.Locator.AppSettings;
        public IDataProvider DataService => App.Locator.DataService;
        public ILoginSwitch LoginSwitch => App.Locator.LoginSwitch;
        public IToastNotificator Toasts => App.Locator.Toasts;
        #endregion


        #region internal

        public void SetParameter(object parameter)
        { IncomingParameter(parameter); }

        public void SendCommand(string commandName, object context = null)
        { IncomingCommand(commandName, context); }
        #endregion

        #region parameter & command

        protected virtual void IncomingParameter(object parameter)
        { }

        protected virtual void IncomingCommand(string commandName, object context)
        { }
        #endregion

        #region activity

        protected void UserInteraction()
        {
            LoginSwitch.ResetTimeout();
        }

        #endregion


    }
}
