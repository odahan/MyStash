using System;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MyStash.Crouton;
using MyStash.Helpers;
using MyStash.Models;
using MyStash.ResX;
using MyStash.Views;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class EditViewModel : StashViewModelBase
    {
        private InfoSheet sheet, originalSheet;
        private bool inEditMode;

        public EditViewModel()
        {
            CancelCommand = new Command(async () =>
                                        {
                                            UserInteraction();
                                            Sheet = originalSheet;
                                            /*await Toasts.Notify(ToastNotificationType.Info, AppResources.EditViewModel_EditViewModel_Action_canceled, AppResources.EditViewModel_EditViewModel_Data_have_not_been_modified, TimeSpan.FromSeconds(2));*/
                                            await Navigation.ModalDismiss();
                                        });
            ValidateCommand = new Command(async () =>
                                          {
                                              UserInteraction();
                                              if (Sheet == null) return;
                                              if (!isSheetOkVerbose()) return;
                                              try
                                              {
                                                  DataService.SaveInfoSheet(Sheet);
                                                  originalSheet = Sheet;
                                                  MessengerInstance.Send(new NotificationMessage(
                                                      InCreationMode ? Utils.GlobalMessages.DataInserted.ToString() : Utils.GlobalMessages.DataModified.ToString()));
                                                  await Navigation.ModalDismiss();
                                                  /*await Toasts.Notify(ToastNotificationType.Success, AppResources.EditViewModel_EditViewModel_Data_saved, AppResources.EditViewModel_EditViewModel_Data_has_been_saved, TimeSpan.FromSeconds(1));*/
                                              }
                                              catch (Exception ex)
                                              {
                                                  await Toasts.Notify(ToastNotificationType.Error, AppResources.EditViewModel_EditViewModel_Can_t_save_data, AppResources.EditViewModel_EditViewModel_Data_have_NOT_been_saved_,
                                                      TimeSpan.FromSeconds(4));
                                                  await DialogService.ShowError(ex, AppResources.EditViewModel_EditViewModel_Data_error, AppResources.DialogService_ShowMessage_Ok, null);
                                              }
                                          }, () => IsModified);
            DeleteCommand = new Command(async () =>
                                        {
                                            UserInteraction();
                                            if (Sheet == null || Sheet.Id < 0) return;
                                            await DialogService.ShowMessage(AppResources.EditViewModel_EditViewModel_Delete_data__Are_you_sure__,
                                                AppResources.EditViewModel_EditViewModel_Confirmation, AppResources.DialogService_ShowMessage_Ok, AppResources.DialogService_ShowMessage_Cancel,
                                                async b =>
                                                      {
                                                          if (!b) return;
                                                          try
                                                          {
                                                              DataService.DeleteinfoSheet(Sheet);
                                                              MessengerInstance.Send(new NotificationMessage(Utils.GlobalMessages.DataDeleted.ToString()));
                                                              /*await
                                                                  Toasts.Notify(ToastNotificationType.Success, AppResources.EditViewModel_EditViewModel_Data_deleted, AppResources.EditViewModel_EditViewModel_Data_has_been_deleted,
                                                                      TimeSpan.FromSeconds(2));*/
                                                              await Navigation.ModalDismiss();
                                                              Sheet = null;
                                                              originalSheet = null;
                                                          }
                                                          catch (Exception ex)
                                                          {
                                                              await
                                                                  Toasts.Notify(ToastNotificationType.Error, AppResources.EditViewModel_EditViewModel_Can_t_delete_data,
                                                                      AppResources.EditViewModel_EditViewModel_Data_have_NOT_been_deleted_,
                                                                      TimeSpan.FromSeconds(4));
                                                              await DialogService.ShowError(ex, AppResources.EditViewModel_EditViewModel_Data_error, AppResources.DialogService_ShowMessage_Ok, null);
                                                          }
                                                      });

                                        }, () => InEditMode);
            MoreTimeCommand = new Command(() =>
                                          {
                                              UserInteraction();
                                              App.Locator.LoginSwitch.ResetTimeout();
                                              Toasts.Notify(ToastNotificationType.Success, AppResources.DetailViewModel_DetailViewModel_More_time,
                                                  string.Format(AppResources.DetailViewModel_DetailViewModel_You_ve_got__0__seconds_more_,
                                                      App.Locator.AppSettings.TimeOutSeconds), TimeSpan.FromSeconds(4));
                                          }, () => IsMoreTimeAvalaible);

            LoginCategoryCommand = new Command(() =>
                                               {
                                                   UserInteraction();
                                                   Sheet.Category = InfoSheet.CategoryFilter.Login;
                                                   // ReSharper disable ExplicitCallerInfoArgument
                                                   RaisePropertyChanged(nameof(IsLoginCategory));
                                                   RaisePropertyChanged(nameof(IsMiscCategory));
                                                   RaisePropertyChanged(nameof(IsNoteCategory));
                                                   // ReSharper restore ExplicitCallerInfoArgument
                                               });
            NoteCategoryCommand = new Command(() =>
                                              {
                                                  UserInteraction();
                                                  Sheet.Category = InfoSheet.CategoryFilter.Note;
                                                  // ReSharper disable ExplicitCallerInfoArgument
                                                  RaisePropertyChanged(nameof(IsLoginCategory));
                                                  RaisePropertyChanged(nameof(IsMiscCategory));
                                                  RaisePropertyChanged(nameof(IsNoteCategory));
                                                  // ReSharper restore ExplicitCallerInfoArgument
                                              });
            MiscCategoryCommand = new Command(() =>
                                              {
                                                  UserInteraction();
                                                  Sheet.Category = InfoSheet.CategoryFilter.Misc;
                                                  // ReSharper disable ExplicitCallerInfoArgument
                                                  RaisePropertyChanged(nameof(IsLoginCategory));
                                                  RaisePropertyChanged(nameof(IsMiscCategory));
                                                  RaisePropertyChanged(nameof(IsNoteCategory));
                                                  // ReSharper restore ExplicitCallerInfoArgument
                                              });
            ProCommand = new Command(() =>
                                     {
                                         UserInteraction();
                                         Sheet.IsPro = true;
                                         // ReSharper disable ExplicitCallerInfoArgument
                                         RaisePropertyChanged(nameof(IsProfessional));
                                         RaisePropertyChanged(nameof(IsPersonal));
                                         // ReSharper restore ExplicitCallerInfoArgument
                                     });

            PersoCommand = new Command(() =>
                                     {
                                         UserInteraction();
                                         Sheet.IsPro = false;
                                         // ReSharper disable ExplicitCallerInfoArgument
                                         RaisePropertyChanged(nameof(IsProfessional));
                                         RaisePropertyChanged(nameof(IsPersonal));
                                         // ReSharper restore ExplicitCallerInfoArgument
                                     });

            CheckPasswordCommand = new Command(() =>
                                               {
                                                   UserInteraction();
                                                   Navigation.ModalNavigateTo(PageName.CheckPw.ToString(), Sheet?.Password);
                                               },
                ()=>!string.IsNullOrWhiteSpace(Sheet?.Password));
            GeneratePasswordCommand = new Command(() =>
                                                  {
                                                      UserInteraction();
                                                      Navigation.ModalNavigateTo(PageName.GeneratePw.ToString(), true);
                                                  });
        }


        protected override void IncomingParameter(object parameter)
        {
            UserInteraction();
            originalSheet = parameter as InfoSheet;
            Sheet = (InfoSheet)originalSheet?.Clone();
            InEditMode = originalSheet != null;
            if (Sheet == null) Sheet = new InfoSheet {Category = InfoSheet.CategoryFilter.Login};
            DeleteCommand.ChangeCanExecute();
            CheckPasswordCommand.ChangeCanExecute();
        }

        public bool IsMoreTimeAvalaible => App.Locator.AppSettings.TimeOutSeconds > 1;

        public bool InEditMode
        {
            get { return inEditMode; }
            set
            {
                UserInteraction();
                Set(ref inEditMode, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(InCreationMode));
            }
        }

        public bool InCreationMode
        {
            get { return !InEditMode; }
            set
            {
                UserInteraction();
                InEditMode = !value;
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(InCreationMode));
            }
        }

        public bool IsSheetOk
        {
            get { return isSheetOk(); }
        }

        public InfoSheet Sheet
        {
            get { return sheet; }
            set
            {
                UserInteraction();
                if (sheet != null) sheet.PropertyChanged -= sheetPropertyChanged;
                Set(ref sheet, value);
                if (sheet != null) sheet.PropertyChanged += sheetPropertyChanged;
                // ReSharper disable ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(IsSheetOk));
                RaisePropertyChanged(nameof(IsModified));
                RaisePropertyChanged(nameof(IsLoginCategory));
                RaisePropertyChanged(nameof(IsNoteCategory));
                RaisePropertyChanged(nameof(IsMiscCategory));
                RaisePropertyChanged(nameof(IsProfessional));
                RaisePropertyChanged(nameof(IsPersonal));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        [LocalizationRequired(false)]
        private void sheetPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged(nameof(IsModified));
            ValidateCommand.ChangeCanExecute();
            if (e.PropertyName== "Password") CheckPasswordCommand.ChangeCanExecute();
        }

        public bool IsModified => Sheet?.IsModified ?? false;

        public bool IsLoginCategory => Sheet?.Category == InfoSheet.CategoryFilter.Login;

        public bool IsNoteCategory => Sheet?.Category == InfoSheet.CategoryFilter.Note;

        public bool IsMiscCategory => Sheet?.Category == InfoSheet.CategoryFilter.Misc;

        public bool IsProfessional => Sheet?.IsPro == true;

        public bool IsPersonal => Sheet?.IsPro == false;

        public Command ValidateCommand { get; }
        public Command CancelCommand { get; }
        public Command DeleteCommand { get; }
        public Command MoreTimeCommand { get; }

        public Command LoginCategoryCommand { get; }
        public Command NoteCategoryCommand { get; }
        public Command MiscCategoryCommand { get; }

        public Command ProCommand { get; }
        public Command PersoCommand { get; }

        public Command CheckPasswordCommand { get; }
        public Command GeneratePasswordCommand { get; }

        private bool isSheetOk()
        {
            if (string.IsNullOrWhiteSpace(Sheet?.Title)) return false;
            if (Sheet?.Category == InfoSheet.CategoryFilter.All) return false;
            if (Sheet?.Category == InfoSheet.CategoryFilter.Login)
            {
                if (string.IsNullOrWhiteSpace(Sheet?.Login)) return false;
                // ReSharper disable once InvertIf
                if (string.IsNullOrWhiteSpace(Sheet?.Password)) return false;
            }
            // ReSharper disable once InvertIf
            if (string.IsNullOrWhiteSpace(Sheet?.Note)) return false;
            return true;
        }


        private bool isSheetOkVerbose()
        {
            try
            {
                if (Sheet == null) return false;
                if (string.IsNullOrWhiteSpace(Sheet.Title))
                {
                    Toasts.Notify(ToastNotificationType.Warning, AppResources.EditViewModel_isSheetOkVerbose_Enter_a_title, AppResources.EditViewModel_isSheetOkVerbose_Title_is_mandatory, TimeSpan.FromSeconds(2));
                    return false;
                }
                if (Sheet.Category == InfoSheet.CategoryFilter.All)
                {
                    Toasts.Notify(ToastNotificationType.Warning, AppResources.EditViewModel_isSheetOkVerbose_Select_a_category, AppResources.EditViewModel_isSheetOkVerbose_A_category_must_be_selected, TimeSpan.FromSeconds(2));
                    return false;
                }
                if (Sheet.Category == InfoSheet.CategoryFilter.Login)
                {
                    if (string.IsNullOrWhiteSpace(Sheet.Login))
                    {
                        Toasts.Notify(ToastNotificationType.Warning, AppResources.EditViewModel_isSheetOkVerbose_Login_is_empty, AppResources.EditViewModel_isSheetOkVerbose_Enter_a_Login_name, TimeSpan.FromSeconds(2));
                        return false;
                    }
                    // ReSharper disable once InvertIf
                    if (string.IsNullOrWhiteSpace(Sheet.Password))
                    {
                        Toasts.Notify(ToastNotificationType.Warning, AppResources.EditViewModel_isSheetOkVerbose_Password_is_empty, AppResources.EditViewModel_isSheetOkVerbose_Enter_a_password, TimeSpan.FromSeconds(2));
                        return false;
                    }
                    return true;
                }
                // ReSharper disable once InvertIf
                if (string.IsNullOrWhiteSpace(Sheet.Note))
                {
                    Toasts.Notify(ToastNotificationType.Warning, AppResources.EditViewModel_isSheetOkVerbose_Note_is_empty, AppResources.EditViewModel_isSheetOkVerbose_Enter_something_in_Note_field, TimeSpan.FromSeconds(2));
                    return false;
                }
                return true;
            }
            finally
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(IsSheetOk));
            }
        }


    }
}
