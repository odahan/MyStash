using GalaSoft.MvvmLight.Messaging;
using MyStash.Helpers;
using MyStash.ResX;
using MyStash.Views;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    class ImportViewModel : StashViewModelBase
    {
        public ImportViewModel()
        {
            CancelButtonCommand = new Command(()=>Navigation.ModalDismiss());
            OkButtonCommand = new Command(()=>
                                          {
                                              MessengerInstance.Send(new NotificationMessage<string>(this, data,
                                                  Utils.GlobalMessages.ImportDataCopied.ToString()));
                                              Navigation.ModalDismiss();
                                          });
        }

        protected override void IncomingParameter(object parameter)
        {
            var s = (ImportViewDataType) parameter;
            mode = s;
        }

        public Command OkButtonCommand { get; }
        public Command CancelButtonCommand { get; }

        private string title;
        private ImportViewDataType mode;
        private string data;

        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        public ImportViewDataType Mode
        {
            get { return mode; }
            set
            {
                if (Set(ref mode, value))
                    Title = string.Format(AppResources.ImportViewModel_Mode__0__Data_Import, mode);
            }
        }

        public string Data
        {
            get { return data; }
            set { Set(ref data, value); }
        }
    }
}
