using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MyStash.Helpers;
using MyStash.Models;
using MyStash.ResX;
using MyStash.Views;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class MainListViewModel : StashViewModelBase
    {
        private GroupedInfoSheetGroups allSheets, loginSheets, noteSheets, miscSheets;
        private bool isBusy;
        private InfoSheet.ProFilter proFilter;
        private int dataCount, loginCount, noteCount, miscCount;
        private Tuple<InfoSheet.ProFilter, string> profilterItem;
        private readonly List<Tuple<InfoSheet.ProFilter, string>> proFilterList;
        private string freeFilter;


        public MainListViewModel()
        {
            proFilterList = InfoSheet.ProfilterTuples;
            ProFilter = InfoSheet.ProFilter.All;
            profilterItem = proFilterList[0];
            SearchCommand = new Command(() => { UserInteraction(); loadData(); });
            LockScreenCommand = new Command(() => LoginSwitch.LogOut());
            ChangePasswordCommand = new Command(() => Navigation.ModalNavigateTo(PageName.SetPwPage.ToString(), true));
            ParametersCommand = new Command(() => Navigation.NavigateTo(PageName.SettingsPage.ToString()));
            NewEntryCommand = new Command(() => Navigation.ModalNavigateTo(PageName.EditPage.ToString()));
            MessengerInstance.Register<NotificationMessage>(this, n =>
                                                                  {
                                                                      if (n.Notification == Utils.GlobalMessages.SettingsChanged.ToString())
                                                                      {
                                                                          // ReSharper disable once ExplicitCallerInfoArgument
                                                                          RaisePropertyChanged(nameof(IsLineVisible));
                                                                          // ReSharper disable once ExplicitCallerInfoArgument
                                                                          RaisePropertyChanged(nameof(IsDateVisible));
                                                                          return;
                                                                      }
                                                                      if (n.Notification == Utils.GlobalMessages.DataDeleted.ToString() ||
                                                                          n.Notification == Utils.GlobalMessages.DataInserted.ToString())
                                                                      {
                                                                          loadData();
                                                                          return;
                                                                      }
                                                                      if (n.Notification == Utils.GlobalMessages.DataModified.ToString())
                                                                      {
                                                                          loadData();
                                                                          return;
                                                                      }
                                                                  });

            loadData();
            if (DataCount == 0) createTest();

        }


        protected override void IncomingCommand(string commandName, object context)
        {
            if (commandName != Utils.GlobalCommands.ListviewTapped.ToString()) return;
            var sheet = context as InfoSheet;
            if (sheet == null) return;
            Navigation.NavigateTo(PageName.DetailPage.ToString(), sheet, false);
        }

        public Command LockScreenCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command ParametersCommand { get; }
        public Command NewEntryCommand { get; }

        [LocalizationRequired(false)]
        [Conditional("DEBUG")]
        private void createTest()
        {
            var i = new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Note,
                Title = "Titre un peu long pour le test",
                Note =
                            "Ceci est une note un peu longue pour tester le comportement de l'affichage quand les notes sont longues et même si c'est embêtant de taper un texte pareil",
                Pro = InfoSheet.ProFilter.Personal
            };
            DataService.SaveInfoSheet(i);

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Note,
                Title = "Note deux pro",
                Note = "note courte de type professionnelle.",
                Pro = InfoSheet.ProFilter.Profesional
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Login,
                UrlOrName = "not an url",
                Title = "Wonder World",
                Login = "titi@toto.com",
                Password = "magicalword",
                Pro = InfoSheet.ProFilter.Personal
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Login,
                Title = "E-Naxos",
                UrlOrName = "http://www.e-naxos.com",
                Login = "tita@glups.com",
                Password = "qsd89",
                Note = "E-Naxos web site",
                Pro = InfoSheet.ProFilter.Personal
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Misc,
                Title = "Saurus",
                Note = "XBC8-88WX-SEDS-777 achetée le 23.01.15",
                Pro = InfoSheet.ProFilter.Profesional
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Misc,
                Title = "Microsoft",
                Note = "8987-888-111-1111 achetée le 30.03.16",
                Pro = InfoSheet.ProFilter.Personal
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Misc,
                Title = "Office",
                Note = "8987-888-111-1111 achetée le 10.01.16",
                Pro = InfoSheet.ProFilter.Profesional
            });

            DataService.SaveInfoSheet(new InfoSheet()
            {
                Category = InfoSheet.CategoryFilter.Misc,
                Title = "Portier manu",
                Note = "58AF",
                Pro = InfoSheet.ProFilter.Personal
            });
            loadData();
        }


        public InfoSheet.ProFilter ProFilter
        {
            get { return proFilter; }
            set
            {
                UserInteraction();
                if (Set(ref proFilter, value)) loadData();
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { Set(ref isBusy, value); }
        }

        private void loadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                DataCount = DataService.GetInfoSheetCount(InfoSheet.CategoryFilter.All, ProFilter).Item1;
                AllSheets = DataService.GetInfoSheetList(InfoSheet.CategoryFilter.All, ProFilter, FreeFilter);
                LoginSheets = DataService.GetInfoSheetList(InfoSheet.CategoryFilter.Login, ProFilter, FreeFilter);
                NoteSheets = DataService.GetInfoSheetList(InfoSheet.CategoryFilter.Note, ProFilter, freeFilter);
                MiscSheets = DataService.GetInfoSheetList(InfoSheet.CategoryFilter.Misc, ProFilter, freeFilter);
                LoginCount = LoginSheets.Sum(g => g.Count);
                NoteCount = NoteSheets.Sum(g => g.Count);
                MiscCount = MiscSheets.Sum(g => g.Count);

            }
            finally
            {
                IsBusy = false;
            }
        }

        public int DataCount
        {
            get { return dataCount; }
            private set
            {
                Set(ref dataCount, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(DataCountStr));
            }
        }

        public int LoginCount
        {
            get { return loginCount; }
            set
            {
                Set(ref loginCount, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(LoginCountStr));
            }
        }

        public int NoteCount
        {
            get { return noteCount; }
            set
            {
                Set(ref noteCount, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(NoteCountStr));
            }
        }

        public int MiscCount
        {
            get { return miscCount; }
            set
            {
                Set(ref miscCount, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(MiscCountStr));
            }
        }

        public string DataCountStr => AppResources.ViewMainList_AllPlural + " " + dataCount;
        public string LoginCountStr => AppResources.ViewMainList_Logins + " " + loginCount;
        public string NoteCountStr => AppResources.ViewMainList_Notes + " " + noteCount;
        public string MiscCountStr => AppResources.ViewMainList_MiscPlural + " " + miscCount;

        public GroupedInfoSheetGroups AllSheets
        {
            get { return allSheets; }
            private set
            {
                allSheets = null;
                RaisePropertyChanged();
                allSheets = value;
                RaisePropertyChanged();

                //Set(ref allSheets, value);
            }
        }

        public GroupedInfoSheetGroups LoginSheets
        {
            get { return loginSheets; }
            private set
            {
                loginSheets = null;
                RaisePropertyChanged();
                loginSheets = value;
                RaisePropertyChanged();
                //Set(ref loginSheets, value);
            }
        }

        public GroupedInfoSheetGroups NoteSheets
        {
            get { return noteSheets; }
            private set
            {
                noteSheets = null;
                RaisePropertyChanged();
                noteSheets = value;
                RaisePropertyChanged();
                //Set(ref noteSheets, value);
            }
        }


        public GroupedInfoSheetGroups MiscSheets
        {
            get { return miscSheets; }
            private set
            {
                miscSheets = null;
                RaisePropertyChanged();
                miscSheets = value;
                RaisePropertyChanged();
                //Set(ref miscSheets, value);
            }
        }

        public List<Tuple<InfoSheet.ProFilter, string>> ProFilterItems => proFilterList;

        public Tuple<InfoSheet.ProFilter, string> ProfilterItem
        {
            get { return profilterItem; }
            set
            {
                UserInteraction();
                if (Set(ref profilterItem, value)) ProFilter = value.Item1;
            }
        }

        public Command SearchCommand { get; private set; }

        public string FreeFilter
        {
            get { return freeFilter; }
            set
            {
                UserInteraction();
                if (!Set(ref freeFilter, value)) return;
                if (string.IsNullOrEmpty(value)) loadData();
            }
        }

        public bool IsLineVisible => Settings.DisplayLinesInLists;

        public bool IsDateVisible => Settings.DisplayDateInLists;
    }
}
