using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using MyStash.Helpers;
using MyStash.Service;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class MainViewModel : StashViewModelBase
    {

        public MainViewModel()
        {
            TapCommand = new Command<string>(s =>
                                                  {
                                                      UserInteraction();
                                                      CurrentValue += s;
                                                      MessengerInstance.Send(new NotificationMessage<string>(s, Utils.GlobalMessages.AnimateDoorButton.ToString()));
                                                  });

        }


        private void logOk()
        {
            currentValue = string.Empty;
            var ls = ServiceLocator.Current.GetInstance<ILoginSwitch>();
            ls?.ShowMainPage();
        }

        private void checkLog()
        {
            if (string.IsNullOrWhiteSpace(CurrentValue)) return;
            if (Settings.GetDoorLock() == CurrentValue)
            {
                logOk();
                return;
            }
            if ((Settings.GetDoorLock()).Substring(0, CurrentValue.Length) != CurrentValue) currentValue = string.Empty;
        }

        public Command<string> TapCommand { get; private set; }


        private string currentValue;
        public string CurrentValue
        {
            get { return currentValue; }
            set { if (Set(ref currentValue, value)) checkLog(); }
        }
       
    }
}
