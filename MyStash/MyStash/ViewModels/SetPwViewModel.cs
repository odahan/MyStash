using System.Linq;
using MyStash.ResX;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class SetPwViewModel : StashViewModelBase
    {

        private string pw1;
        private string pw2;
        private bool isValid;
        private string lastError;

        public SetPwViewModel()
        {
            OkCommand = new Command(async () =>
                                         {
                                             UserInteraction();
                                             if (!IsValid) return;
                                             Settings.SetDoorLock(Pw1);
                                             await DialogService.ShowMessage(
                                                 string.Format(AppResources.SetPwViewModel_SetPwViewModel_Your_new_password___0___has_been_recorded__Don_t_forget_it__, pw1), AppResources.SetPwViewModel_SetPwViewModel_Confirmation);
                                             if (isChangingPW)
                                             {
                                                 await Navigation.ModalDismiss();
                                                 return;
                                             }
                                             LoginSwitch.LogOut();
                                         },
                                       () => IsValid);

            CancelCommand = new Command(async () =>
                                        {
                                            UserInteraction();
                                            if (isChangingPW)
                                            {
                                                await Navigation.ModalDismiss();
                                                return;
                                            }
                                            LoginSwitch.LogOut();
                                        });
        }

        public Command OkCommand { get; private set; }
        public Command CancelCommand { get; private set; }

        private bool isChangingPW;

        protected override void IncomingParameter(object parameter)
        {
            if (parameter is bool)
            {
                isChangingPW = (bool)parameter;
                Pw1 = string.Empty;
                Pw2 = string.Empty;
            }
        }

        public string Pw1
        {
            get { return pw1; }
            set
            {
                UserInteraction();
                Set(ref pw1, value?.ToUpper());
                checkValidity();
            }
        }

        public string Pw2
        {
            get { return pw2; }
            set
            {
                UserInteraction();
                Set(ref pw2, value?.ToUpper());
                checkValidity();
            }
        }

        public string LastError
        {
            get { return lastError; }
            set { Set(ref lastError, value); }
        }


        private void checkValidity()
        {
            IsValid =
                !string.IsNullOrWhiteSpace(Pw1) &&
                (Pw1.Length >= 4) &&
                (Pw1.Length <= 20) &&
                allCharsAllowed(Pw1) &&
                Pw1 == Pw2 &&
                pw1.ToCharArray().Distinct().Count() != 1;
            LastError = "";
            if (string.IsNullOrWhiteSpace(Pw1))
            {
                LastError = AppResources.SetPwViewModel_checkValidity_Password_can_t_be_empty_;
                return;
            }
            if (!allCharsAllowed(Pw1))
            {
                LastError = AppResources.SetPwViewModel_checkValidity_Invalid_character_s__in_password_;
                return;
            }
            if (pw1 != pw2)
            {
                LastError = AppResources.SetPwViewModel_checkValidity_Password_and_control_don_t_match__;
                return;
            }
            if (Pw1.Length < 4)
            {
                LastError = AppResources.SetPwViewModel_checkValidity_;
                return;
            }
            if (Pw1.Length > 20)
                LastError = AppResources.SetPwViewModel_checkValidity_20;
            if (pw1.ToCharArray().Distinct().Count() == 1)
                LastError = AppResources.SetPwViewModel_checkValidity_Only_one_single_character_is_used;
        }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (Set(ref isValid, value))
                    OkCommand.ChangeCanExecute();
            }
        }

        private static bool allCharsAllowed(string s)
        {
            const string allowed = "ABCDEF0123456789";
            return !string.IsNullOrWhiteSpace(s) && s.ToUpper().Cast<char>().All(c => allowed.Contains(c.ToString()));
        }
    }
}
