using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MyStash.Crouton;
using MyStash.Export;
using MyStash.Helpers;
using MyStash.Models;
using MyStash.ResX;
using MyStash.Service;
using MyStash.Views;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class SettingsViewModel : StashViewModelBase
    {
        public SettingsViewModel()
        {
            Languages =
                AppSettings.CultureDictionary.Select(pair => new Tuple<SupportedCulture, string>(pair.Key, pair.Value))
                    .OrderBy(tuple => (int)tuple.Item1)
                    .ToList();
            DataCommand = new Command<string>(s =>
                                              {
                                                  switch (s)
                                                  {
                                                      case "0":
                                                          exportCsv();
                                                          break;
                                                      case "1":
                                                          exportSql();
                                                          break;
                                                      case "2":
                                                          importCsv();
                                                          break;
                                                      case "3":
                                                          importSql();
                                                          break;
                                                  }
                                              }

                );
            MessengerInstance.Register<NotificationMessage<string>>(this, n =>
                                                                          {
                                                                              if (n.Notification != Utils.GlobalMessages.ImportDataCopied.ToString()) return;
                                                                              if (string.IsNullOrWhiteSpace(n.Content)) return;
                                                                              importCSVWithData(n.Content);
                                                                          });
            originalData = DataService.GetAllSheets();
            var pfl = AppSettings.PreferredCulture;
            var x = Languages.FirstOrDefault(tuple => tuple.Item1 == pfl);
            SelectedCultureItem = x;
        }

        private bool encryptedData = true;
        private bool skipExistingTitle = true;
        private bool skipExistingId = true;
        private bool clearDbBeforeImport;
        private bool shareOnExport;
        private readonly List<InfoSheet> originalData;
        private Tuple<SupportedCulture, string> selectedCultureItem;

        public IAppSettings AppSettings => Locator.AppSettings;

        public List<Tuple<SupportedCulture, string>> Languages { get; }

        public Tuple<SupportedCulture, string> SelectedCultureItem
        {
            get { return selectedCultureItem; }
            set
            {
                Set(ref selectedCultureItem, value);
                AppSettings.PreferredCulture = value.Item1;
            }
        }

        public bool EncryptedData
        {
            get { return encryptedData; }
            set
            {
                if (Set(ref encryptedData, value) && value == false)
                    DialogService.ShowMessage(AppResources.SettingsViewModel_EncryptedData_Uncrypted_data_export_is_not_safe, AppResources.SettingsViewModel_EncryptedData_Warning);
            }
        }

        public bool ShareOnExport
        {
            get { return shareOnExport; }
            set { Set(ref shareOnExport, value); }
        }

        public bool SkipExistingTitle
        {
            get { return skipExistingTitle; }
            set { Set(ref skipExistingTitle, value); }
        }

        public bool SkipExistingId
        {
            get { return skipExistingId; }
            set { Set(ref skipExistingId, value); }
        }


        public bool ClearDbBeforeImport
        {
            get { return clearDbBeforeImport; }
            set
            {
                if (Set(ref clearDbBeforeImport, value) && value)
                    DialogService.ShowMessage(AppResources.SettingsViewModel_ClearDbBeforeImport, AppResources.SettingsViewModel_EncryptedData_Warning);
            }
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Command<string> DataCommand { get; }

        [LocalizationRequired(false)]
        private void exportData(bool sql = false)
        {
            if (DataService.GetInfoSheetCount(InfoSheet.CategoryFilter.All, InfoSheet.ProFilter.All).Item1 < 1)
            {
                Toasts.Notify(ToastNotificationType.Warning, AppResources.SettingsViewModel_exportData_No_data, AppResources.SettingsViewModel_exportData_No_data_to_export, TimeSpan.FromSeconds(3));
                return;
            }
            try
            {
                var list = originalData;
                if (EncryptedData) list = (from insh in originalData select insh.ToCryptedSheet()).ToList();

                ExportBase<InfoSheet> exp;
                if (sql) exp = new ExportClipboardSql<InfoSheet>();
                else exp = new ExportClipboardCsv<InfoSheet>() { TitleRow = true };
                exp.Source = list;
                foreach (var fi in exp.FieldsInformation)
                    fi.Value.IsExported = false;
                exp.FieldsInformation["Id"].IsExported =
                    exp.FieldsInformation["Title"].IsExported =
                        exp.FieldsInformation["IsPro"].IsExported =
                            exp.FieldsInformation["Note"].IsExported =
                                exp.FieldsInformation["CreatedOn"].IsExported =
                                    exp.FieldsInformation["ModifiedOn"].IsExported =
                                        exp.FieldsInformation["UrlOrName"].IsExported =
                                            exp.FieldsInformation["Login"].IsExported =
                                                exp.FieldsInformation["Password"].IsExported =
                                                    exp.FieldsInformation["Category"].IsExported =
                                                        exp.FieldsInformation["Crypting"].IsExported = true;
                exp.Destination = ShareOnExport ? Destination.Share : Destination.Clipboard;
                exp.Execute();
                Toasts.Notify(ToastNotificationType.Success, (sql ? "SQL" : "CSV") + " " + AppResources.SettingsViewModel_exportData_Export, AppResources.SettingsViewModel_exportData_Data_are_in_the_clipboard, TimeSpan.FromSeconds(2));
            }
            catch (Exception ex)
            {
                DialogService.ShowError(ex, AppResources.SettingsViewModel_exportData_Error, AppResources.SettingsViewModel_exportData_OK, null);
            }
        }

        private void exportCsv()
        {
            exportData();
        }

        [LocalizationRequired(false)]
        private void exportSql()
        {
            exportData(true);
        }

        [LocalizationRequired(false)]
        private void importCsv()
        {
           Navigation.ModalNavigateTo(PageName.DataImportPage.ToString(), ImportViewDataType.CSV);
        }

        [LocalizationRequired(false)]
        private void importCSVWithData(string data)
        {
            data = data.Replace("\r\n", "\n");
            var s = data.Split('\n').ToList();
            s = s.Select(x => x.NoControlAndTrim()).ToList();
            int i;
            try
            {
                i = csvImport(s);
            }
            catch (Exception ex)
            {
                i = 0;
                DialogService.ShowError(ex, AppResources.SettingsViewModel_exportData_Error, AppResources.SettingsViewModel_exportData_OK, null);
            }
            if (i > 0) Toasts.Notify(ToastNotificationType.Success, AppResources.SettingsViewModel_importCSVWithData_Import_succeed, string.Format(AppResources.SettingsViewModel_importCSVWithData__0__sheets_have_been_imported_,i), TimeSpan.FromSeconds(3));
            if (i < 1) Toasts.Notify(ToastNotificationType.Info, AppResources.SettingsViewModel_importCSVWithData_No_data_imported, AppResources.SettingsViewModel_importCSVWithData_No_data_have_been_imported_, TimeSpan.FromSeconds(3));
        }

        private void importSql()
        {
            Toasts.Notify(ToastNotificationType.Info, AppResources.SettingsViewModel_importSql_Not_implemented, AppResources.SettingsViewModel_importSql_No_SQL_import_in_this_version, TimeSpan.FromSeconds(3));
        }

        [LocalizationRequired(false)]
        private int csvImport(IEnumerable<string> data)
        {
            data = data.Where(s => !string.IsNullOrWhiteSpace(s.Trim())).ToList();
            // parse
            var lines = new List<string[]>();
            var curline = 0;
            foreach (var s in data)
            {
                curline++;
                var k = s.Split(';');
                if (k.Length < 1)
                {
                    Toasts.Notify(ToastNotificationType.Error, AppResources.SettingsViewModel_csvImport_Import_failed, string.Format(AppResources.SettingsViewModel_csvImport_Import_aborted_bad_data_line__0_,curline + 1), TimeSpan.FromSeconds(3));
                    return 0;
                }
                var kk = new string[k.Length];
                var i = 0;
                foreach (var fragment in k)
                {
                    var f = fragment.Trim();
                    f = f.Trim('"');
                    f = f.Replace("\"\"", "\"");
                    kk[i++] = f;
                }
                lines.Add(kk);
            }
            // test 1st line
            var fieldPositions = new List<fieldInfo>();
            var pos = 0;
            var lineZero = lines[0];
            foreach (var item in lineZero)
            {
                var p = fieldNames.FirstOrDefault(fn => string.Compare(fn.Name, item, StringComparison.OrdinalIgnoreCase) == 0);
                if (p.OriginalPos < 0)
                {
                    Toasts.Notify(ToastNotificationType.Error, AppResources.SettingsViewModel_exportData_Error, string.Format(AppResources.SettingsViewModel_csvImport_field__0__can_t_be_found, item), TimeSpan.FromSeconds(3));
                    return 0;
                }
                p.Pos = pos++;
                fieldPositions.Add(p);
            }
            fieldPositions = fieldPositions.OrderBy(fp => fp.Pos).ToList();
            if (fieldPositions.Any(fp => fp.Pos != fp.OriginalPos) || fieldPositions.Count != fieldNames.Count)
            {
                Toasts.Notify(ToastNotificationType.Error, AppResources.SettingsViewModel_exportData_Error, AppResources.SettingsViewModel_csvImport_Corrupted_data__1st_line_missing_, TimeSpan.FromSeconds(3));
                return 0;
            }
            // make a list of sheets
            var list = new List<InfoSheet>(lines.Count);
            lines.RemoveAt(0); // titles are no more useful
            foreach (var line in lines)
            {
                var fcInt = new Func<string, int>(s =>
                                                  {
                                                      int i;
                                                      var b = int.TryParse(s, out i);
                                                      if (!b) throw new Exception(string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_ID, s));
                                                      return i;
                                                  });
                var fcBool = new Func<string, bool>(s =>
                                                    {
                                                        bool i;
                                                        var b = bool.TryParse(s, out i);
                                                        if (!b) throw new Exception(string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_boolean, s));
                                                        return i;
                                                    });
                var fcDateTime = new Func<string, DateTime>(s =>
                                                            {
                                                                DateTime i;
                                                                var b = DateTime.TryParse(s, out i);
                                                                if (!b) throw new Exception(string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_date_time, s));
                                                                return i;
                                                            });
                var fcCat = new Func<string, InfoSheet.CategoryFilter>(s =>
                                                                       {
                                                                           try
                                                                           {
                                                                               return s.ToEnum(InfoSheet.CategoryFilter.Misc);
                                                                           }
                                                                           catch
                                                                           {
                                                                               throw new Exception(string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_Category, s));
                                                                           }
                                                                       });
                var fcAlgo = new Func<string, SheetCrypting>(s =>
                                                             {
                                                                 try
                                                                 {
                                                                     return s.ToEnum(SheetCrypting.None);
                                                                 }
                                                                 catch
                                                                 {
                                                                     throw new Exception(
                                                                         string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_algorithm_data_could_be_unreadable, s));
                                                                 }
                                                             });

                var sheet = new InfoSheet();
                var modified = DateTime.Now;
                foreach (var p in fieldPositions)
                {
                    var str = line[p.Pos];
                    switch (p.OriginalPos)
                    {
                        case 0:
                            sheet.Id = fcInt(str);
                            break;
                        case 1:
                            sheet.Title = str;
                            break;
                        case 2:
                            sheet.IsPro = fcBool(str);
                            break;
                        case 3:
                            sheet.Note = str;
                            break;
                        case 4:
                            sheet.CreatedOn = fcDateTime(str);
                            break;
                        case 5:
                            modified = fcDateTime(str);
                            break;
                        case 6:
                            sheet.UrlOrName = str;
                            break;
                        case 7:
                            sheet.Login = str;
                            break;
                        case 8:
                            sheet.Password = str;
                            break;
                        case 9:
                            sheet.Category = fcCat(str);
                            break;
                        case 10:
                            sheet.Crypting = fcAlgo(str);
                            break;
                        default:
                            throw new Exception(string.Format(AppResources.SettingsViewModel_csvImport___0___is_not_a_valid_field_position_, p.OriginalPos));
                    }
                }
                sheet.ModifiedOn = modified;
                sheet.IsModified = false;
                sheet = sheet.ToUnencryptedSheet(); // will be uncrypted by save method
                list.Add(sheet);
            }

            // import data

            try
            {
                 var t = DataService.ImportData(list, SkipExistingId, SkipExistingTitle, ClearDbBeforeImport);
                 if (t.Item2 > 0 || t.Item3 > 0)
                     Toasts.Notify(ToastNotificationType.Info, AppResources.SettingsViewModel_csvImport_Ignored_data,
                         string.Format(AppResources.SettingsViewModel_csvImport__0__sheets_imported___1__ID_skipped___2__Title_skipped_, t.Item1, t.Item2, t.Item3), TimeSpan.FromSeconds(5));
                 MessengerInstance.Send(new NotificationMessage(Utils.GlobalMessages.DataInserted.ToString()));
                 return t.Item1;
  }
            catch (Exception ex)
            {
                DialogService.ShowError(ex, AppResources.SettingsViewModel_exportData_Error, AppResources.SettingsViewModel_exportData_OK, null);
                return -1;
            }
        }

        private class fieldInfo
        {
            public string Name;
            public int Pos = -1;
            public readonly int OriginalPos;

            public fieldInfo(string name, int originalPos)
            {
                Name = name;
                OriginalPos = originalPos;
            }

            public static fieldInfo New(string name, int originalPos)
            {
                return new fieldInfo(name, originalPos);
            }
        }

        [LocalizationRequired(false)]
        private readonly List<fieldInfo> fieldNames = new List<fieldInfo>
                                             {
                                                 fieldInfo.New("Id", 0),
                                                 fieldInfo.New("Title", 1),
                                                 fieldInfo.New("IsPro", 2),
                                                 fieldInfo.New("Note", 3),
                                                 fieldInfo.New("CreatedOn", 4),
                                                 fieldInfo.New("ModifiedOn", 5),
                                                 fieldInfo.New("UrlOrName", 6),
                                                 fieldInfo.New("Login", 7),
                                                 fieldInfo.New("Password", 8),
                                                 fieldInfo.New("Category", 9),
                                                 fieldInfo.New("Crypting", 10)
                                             };
    };
}

/*
"Id";"Title";"IsPro";"Note";"CreatedOn";"ModifiedOn";"UrlOrName";"Login";"Password";"Category";"Crypting"
1;"Titre un peu long pour le test";"False";"DwQBDExWQStnEBomTwIABk0SQEISQElBEEVOSkAnAFAVDgFSUBgAEgcAVW4DIUEXDjkCFjMTBAQLIhVCAQkTXngmAxIqDAQOFU0SRFlTXkgUXExSBEk9ERUWQQdPHhhFDRwLQDsKN0ERFXQfwpMsAkEaB2wCRQAfRxI6KgfCnjcOAhtSTFcVWFNASUYQXE8EUzcdBABBBEECCQwN";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Note";"Algo1"
2;"Note deux pro";"True";"Ig4WAExQXSo1ERFjCwlPBlFCUAxCQkNSVVpSTUg8CxUJDREO";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Note";"Algo1"
3;"Wonder World";"False";"";2016/07/29 08:58:40;2016/07/29 08:58:40;"Ig4WRQ1dEio1CQ==";"OAgWDCxHXSsoSxcsAg==";"IQAFDA9SXigoFxA=";"Login";"Algo1"
4;"E-Naxos";"False";"CUwsBBRcQX8wABZjHAUbFw==";2016/07/29 08:58:40;2016/07/29 08:58:40;"JBUWFVYcHSgwElomQgIOCkdBG09dXQ==";"OAgWBCxUXio3FlogAAE=";"PRIGXVU=";"Login";"Algo1"
5;"Saurus";"True";"FCMhXUELCggfSCcGKz9CRR8FFU1RWElAw5lMAUhCcldDS1FFDkFZ";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Misc";"Algo1"
6;"Microsoft";"False";"dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHYVVeVVJaEUY=";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Misc";"Algo1"
7;"Office";"True";"dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHY1VeVVBaEUY=";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Misc";"Algo1"
8;"Portier manu";"False";"eVkjIw==";2016/07/29 08:58:40;2016/07/29 08:58:40;"";"";"";"Misc";"Algo1"

          var l = new List<string>
                    {
                        "\"Id\"; \"Title\"; \"IsPro\"; \"Note\"; \"CreatedOn\"; \"ModifiedOn\"; \"UrlOrName\"; \"Login\"; \"Password\"; \"Category\"; \"Crypting\"",
                        "1; \"Titre un peu long pour le test\"; \"False\"; \"DwQBDExWQStnEBomTwIABk0SQEISQElBEEVOSkAnAFAVDgFSUBgAEgcAVW4DIUEXDjkCFjMTBAQLIhVCAQkTXngmAxIqDAQOFU0SRFlTXkgUXExSBEk9ERUWQQdPHhhFDRwLQDsKN0ERFXQfwpMsAkEaB2wCRQAfRxI6KgfCnjcOAhtSTFcVWFNASUYQXE8EUzcdBABBBEECCQwN\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Note\"; \"Algo1\"",
                        "2; \"Note deux pro\"; \"True\"; \"Ig4WAExQXSo1ERFjCwlPBlFCUAxCQkNSVVpSTUg8CxUJDREO\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Note\"; \"Algo1\"",
                        "3; \"Wonder World\"; \"False\"; \"\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"Ig4WRQ1dEio1CQ==\"; \"OAgWDCxHXSsoSxcsAg==\"; \"IQAFDA9SXigoFxA=\"; \"Login\"; \"Algo1\"",
                        "4; \"E-Naxos\"; \"False\"; \"CUwsBBRcQX8wABZjHAUbFw==\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"JBUWFVYcHSgwElomQgIOCkdBG09dXQ==\"; \"OAgWBCxUXio3FlogAAE=\"; \"PRIGXVU=\"; \"Login\"; \"Algo1\"",
                        "5; \"Saurus\"; \"True\"; \"FCMhXUELCggfSCcGKz9CRR8FFU1RWElAw5lMAUhCcldDS1FFDkFZ\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Misc\"; \"Algo1\"",
                        "6; \"Microsoft\"; \"False\"; \"dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHYVVeVVJaEUY=\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Misc\"; \"Algo1\"",
                        "7; \"Office\"; \"True\"; \"dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHY1VeVVBaEUY=\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Misc\"; \"Algo1\"",
                        "8; \"Portier manu\"; \"False\"; \"eVkjIw==\"; 2016 / 07 / 29 08:58:40; 2016 / 07 / 29 08:58:40; \"\"; \"\"; \"\"; \"Misc\"; \"Algo1\""
                    };
            importCSVWithData(string.Join(Environment.NewLine, l));        




-- Generated By MyStash - 30/07/2016 22:07:56 
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(1, 'Titre un peu long pour le test', 'False', 'DwQBDExWQStnEBomTwIABk0SQEISQElBEEVOSkAnAFAVDgFSUBgAEgcAVW4DIUEXDjkCFjMTBAQLIhVCAQkTXngmAxIqDAQOFU0SRFlTXkgUXExSBEk9ERUWQQdPHhhFDRwLQDsKN0ERFXQfwpMsAkEaB2wCRQAfRxI6KgfCnjcOAhtSTFcVWFNASUYQXE8EUzcdBABBBEECCQwN', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Note', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(2, 'Note deux pro', 'True', 'Ig4WAExQXSo1ERFjCwlPBlFCUAxCQkNSVVpSTUg8CxUJDREO', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Note', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(3, 'Wonder World', 'False', '', 2016/07/29 08:58:40, 2016/07/29 08:58:40, 'Ig4WRQ1dEio1CQ==', 'OAgWDCxHXSsoSxcsAg==', 'IQAFDA9SXigoFxA=', 'Login', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(4, 'E-Naxos', 'False', 'CUwsBBRcQX8wABZjHAUbFw==', 2016/07/29 08:58:40, 2016/07/29 08:58:40, 'JBUWFVYcHSgwElomQgIOCkdBG09dXQ==', 'OAgWBCxUXio3FlogAAE=', 'PRIGXVU=', 'Login', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(5, 'Saurus', 'True', 'FCMhXUELCggfSCcGKz9CRR8FFU1RWElAw5lMAUhCcldDS1FFDkFZ', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Misc', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(6, 'Microsoft', 'False', 'dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHYVVeVVJaEUY=', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Misc', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(7, 'Office', 'True', 'dFhaUkELCmdqVEVyQl1eQxkSVE9aVVjDnVUJTUEHY1VeVVBaEUY=', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Misc', 'Algo1');
INSERT INTO InfoSheet 
([Id], [Title], [IsPro], [Note], [CreatedOn], [ModifiedOn], [UrlOrName], [Login], [Password], [Category], [Crypting]) 
VALUES 
(8, 'Portier manu', 'False', 'eVkjIw==', 2016/07/29 08:58:40, 2016/07/29 08:58:40, '', '', '', 'Misc', 'Algo1');
-- there are 8 records in this file. 



*/
