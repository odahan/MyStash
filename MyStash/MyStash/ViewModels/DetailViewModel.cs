using System;
using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MyStash.Crouton;
using MyStash.Helpers;
using MyStash.Models;
using MyStash.ResX;
using MyStash.Views;
using Plugin.Share;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class DetailViewModel :StashViewModelBase
    {
        private InfoSheet sheet;

        [LocalizationRequired(false)]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        protected override void IncomingParameter(object parameter)
        {
            UserInteraction();
            sheet = parameter as InfoSheet;
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Id));
            RaisePropertyChanged(nameof(IsLoginCategory));
            RaisePropertyChanged(nameof(IsMiscCategory));
            RaisePropertyChanged(nameof(IsNoteCategory));
            RaisePropertyChanged(nameof(IsPersonal));
            RaisePropertyChanged(nameof(IsProfessional));
            RaisePropertyChanged(nameof(Category));
            RaisePropertyChanged(nameof(Login));
            RaisePropertyChanged(nameof(UrlOrName));
            RaisePropertyChanged(nameof(Password));
            RaisePropertyChanged(nameof(CreatedOnStr));
            RaisePropertyChanged(nameof(ModifiedOnStr));
            RaisePropertyChanged(nameof(Note));
            RaisePropertyChanged(nameof(IsNoteVisible));
            RaisePropertyChanged(nameof(IsMoreTimeAvalaible));
            RaisePropertyChanged(nameof(IsCheckPasswordVisible));
            CheckPasswordCommand.ChangeCanExecute();
            CopyPasswordCommand.ChangeCanExecute();
            CopyLoginCommand.ChangeCanExecute();
            CopyNoteCommand.ChangeCanExecute();
            GoToWebCommand.ChangeCanExecute();
            MoreTimeCommand.ChangeCanExecute();
            EditCommand.ChangeCanExecute();

        }

        public DetailViewModel()
        {
            CheckPasswordCommand = new Command(async ()=>await Navigation.ModalNavigateTo(PageName.CheckPw.ToString(),sheet.Password),()=>IsCheckPasswordVisible);
            CopyLoginCommand = new Command(async() =>
                                           {
                                               UserInteraction();
                                               await CrossShare.Current.SetClipboardText(Login);
                                               await Toasts.Notify(ToastNotificationType.Success, AppResources.DetailViewModel_DetailViewModel_Login_copied_to_clipboard,AppResources.DetailViewModel_DetailViewModel_The_login_has_been_copied_to_the_clipboard, TimeSpan.FromSeconds(2));
                                           },()=>!string.IsNullOrWhiteSpace(Login));
            CopyPasswordCommand = new Command(async () =>
                                              {
                                                  UserInteraction();
                                                  await CrossShare.Current.SetClipboardText(Password);
                                                  await Toasts.Notify(ToastNotificationType.Success, AppResources.DetailViewModel_DetailViewModel_Password_copied_to_clipboard, AppResources.DetailViewModel_DetailViewModel_The_password_has_been_copied_to_the_clipboard, TimeSpan.FromSeconds(2));
                                              }, ()=>!string.IsNullOrWhiteSpace(Password));
            CopyNoteCommand = new Command(() =>
            {
                UserInteraction();
                CrossShare.Current.SetClipboardText(Note);
                Toasts.Notify(ToastNotificationType.Success, AppResources.DetailViewModel_DetailViewModel_Note_copied_to_clipboard, AppResources.DetailViewModel_DetailViewModel_The_note_has_been_copied_to_the_clipboard, TimeSpan.FromSeconds(2));
            }, () => !string.IsNullOrWhiteSpace(Note));
            GoToWebCommand = new Command(async () =>
                                         {
                                             UserInteraction();
                                             try
                                             {
                                                 Device.OpenUri(new Uri(UrlOrName, UriKind.RelativeOrAbsolute));
                                             }
                                             catch 
                                             {
                                                 await Toasts.Notify(ToastNotificationType.Error, AppResources.DetailViewModel_DetailViewModel_Invalid_URL,
                                                     AppResources.DetailViewModel_DetailViewModel_Perhaps_are_you_missing_the_protocol_name___http_____for_example___, TimeSpan.FromSeconds(3));
                                             }
                                         }, () =>
                                                                                 {
                                                                                     try
                                                                                     {
                                                                                         Uri.IsWellFormedUriString(UrlOrName,
                                                                                             UriKind.RelativeOrAbsolute);
                                                                                         return true;
                                                                                     }
                                                                                     catch
                                                                                     {
                                                                                         return false;
                                                                                     }
                                                                                 });
            MoreTimeCommand = new Command(async () =>
                                          {
                                              UserInteraction();
                                              App.Locator.LoginSwitch.ResetTimeout();
                                              await Toasts.Notify(ToastNotificationType.Success, AppResources.DetailViewModel_DetailViewModel_More_time,
                                                  string.Format(AppResources.DetailViewModel_DetailViewModel_You_ve_got__0__seconds_more_, App.Locator.AppSettings.TimeOutSeconds), TimeSpan.FromSeconds(4));
                                          },() => IsMoreTimeAvalaible);
            EditCommand = new Command(async () =>
                                      {
                                          UserInteraction();
                                          await Navigation.ModalNavigateTo(PageName.EditPage.ToString(), sheet);
                                      });
            MessengerInstance.Register<NotificationMessage>(this, async n =>
                                                                  {
                                                                      UserInteraction();
                                                                      if (n.Notification == Utils.GlobalMessages.DataModified.ToString())
                                                                      {
                                                                          var s = DataService.GetInfoSheet(sheet.Id);
                                                                          IncomingParameter(s);
                                                                          return;
                                                                      }
                                                                      if (n.Notification == Utils.GlobalMessages.DataDeleted.ToString())
                                                                      {
                                                                          await Navigation.GoBack();
                                                                          return;
                                                                      }
                                                                  });

        }


        public Command CheckPasswordCommand { get; }
        public Command CopyLoginCommand { get; }
        public Command CopyPasswordCommand { get; }
        public Command CopyNoteCommand { get; }
        public Command GoToWebCommand { get; }
        public Command MoreTimeCommand { get; }
        public Command EditCommand { get; }

        public bool IsMoreTimeAvalaible => App.Locator.AppSettings.TimeOutSeconds > 1;

        public string Title => sheet?.Title;

        public int Id => sheet?.Id??-1;

        public bool IsLoginCategory => sheet?.Category == InfoSheet.CategoryFilter.Login;

        public bool IsNoteCategory => sheet?.Category == InfoSheet.CategoryFilter.Note;

        public bool IsMiscCategory => sheet?.Category == InfoSheet.CategoryFilter.Misc;

        public bool IsProfessional => sheet?.IsPro == true;

        public bool IsPersonal => sheet?.IsPro == false;

        public string Category => InfoSheet.CategoryDictionary[sheet?.Category ?? InfoSheet.CategoryFilter.All];

        public string UrlOrName => sheet?.UrlOrName;

        public string Login => sheet?.Login;

        public string Password => sheet?.Password;

        public string CreatedOnStr => AppResources.VMDetail_CreatedOn+" "+sheet?.CreatedOnStr;

        public string ModifiedOnStr => AppResources.VMDetail_ModifiedOn+" "+sheet?.ModifiedOnStr;

        public string Note => sheet?.Note;

        public bool IsNoteVisible => !string.IsNullOrWhiteSpace(sheet?.Note);

        public bool IsCheckPasswordVisible => !string.IsNullOrEmpty(sheet?.Password);
    }
}
