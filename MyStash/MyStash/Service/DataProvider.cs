using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Practices.ServiceLocation;
using MyStash.Models;
using SQLite.Net;
using Xamarin.Forms;
using MyStash.Helpers;

namespace MyStash.Service
{
    public class DataProvider : IDataProvider
    {
        [LocalizationRequired(false)]
        private const string DatabaseName = "Stash.db";
        private static readonly object locker = new object();
        private static bool firstTime=true;

        private SQLiteConnection openConnexion()
        {
            var connection =
                DependencyService.Get<ISqLite>().GetConnection(DatabaseName);
            if (firstTime)
            {
                connection.CreateTable<InfoSheet>();
                connection.MigrateTable<InfoSheet>(); // new fields are added
                connection.CreateTable<AppSettingStorageItem>();
                connection.MigrateTable<AppSettingStorageItem>(); // new fields are added
                firstTime = false;
            }
            return connection;
        }



        #region Info sheets

        [LocalizationRequired(false)]
        public GroupedInfoSheetGroups GetInfoSheetList(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter, string filter = null)
        {
            const string symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ?";
            var gg = new GroupedInfoSheetGroups();
            var groups = (from char c in symbols
                          let cs = c.ToString()
                          select new InfoSheetGroup(cs, cs)).ToList();
            var data = getAllSheetsForCategory(category, proFilter, filter);
            foreach (var d in data)
            {
                var fc = string.IsNullOrWhiteSpace(d.Title) ? "?" : d.Title[0].ToString().ToUpper();
                if (!symbols.Contains(fc)) fc = "?";
                var g = groups.FirstOrDefault(group => @group.GroupShortName == fc) ?? groups[groups.Count - 1];
                g.Add(d);
            }
            for (var i = groups.Count - 1; i >= 0; i--)
                if (groups[i].Count == 0) groups.RemoveAt(i);
            gg.AddRange(groups);
            return gg;
        }

        [LocalizationRequired(false)]
        private string getQuery(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter, bool forCount)
        {
            string pro;
            switch (proFilter)
            {
                case InfoSheet.ProFilter.All:
                    pro = $"{(int)InfoSheet.ProFilter.Profesional},{(int)InfoSheet.ProFilter.Personal}";
                    break;
                case InfoSheet.ProFilter.Profesional:
                    pro = $"{(int)InfoSheet.ProFilter.Profesional}"; ;
                    break;
                case InfoSheet.ProFilter.Personal:
                    pro = $"{(int)InfoSheet.ProFilter.Personal}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(proFilter), proFilter, null);
            }
            string cat;
            switch (category)
            {
                case InfoSheet.CategoryFilter.All:
                    cat = $"{(int)InfoSheet.CategoryFilter.Login},{(int)InfoSheet.CategoryFilter.Note},{(int)InfoSheet.CategoryFilter.Misc}";
                    break;
                case InfoSheet.CategoryFilter.Login:
                    cat = $"{(int)InfoSheet.CategoryFilter.Login}";
                    break;
                case InfoSheet.CategoryFilter.Note:
                    cat = $"{(int)InfoSheet.CategoryFilter.Note}";
                    break;
                case InfoSheet.CategoryFilter.Misc:
                    cat = $"{(int)InfoSheet.CategoryFilter.Misc}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
            var cols = forCount ? "COUNT(*)" : "*";
            var query = $"SELECT {cols} FROM [InfoSheet] WHERE [PRO] IN({pro}) AND [Category] IN({cat}) ORDER BY [Title]";
            return query;
        }

        private IEnumerable<InfoSheet> getAllSheetsForCategory(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter, string filter = null)
        {
            var aps = ServiceLocator.Current.GetInstance<IAppSettings>();
            lock (locker)
            {
                InfoSheet.Loading = true;
                try
                {
                    using (var db = openConnexion())
                    {
                        var sheets = db.Query<InfoSheet>(getQuery(category, proFilter, false)).Select(s =>
                                                                                                      {
                                                                                                          var x = s.ToUnencryptedSheet();
                                                                                                          x.IsModified = false;
                                                                                                          return x;
                                                                                                      });
                        // no filter
                        if (string.IsNullOrWhiteSpace(filter)) return sheets.ToList();

                        // filter on title
                        if (!aps.SearchInNote && !aps.SearchEverywhere)
                            return sheets.Where(s => s.Title.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();

                        // filter on title and note
                        if (aps.SearchInNote && !aps.SearchEverywhere)
                            return sheets.Where(s => s.Title.Contains(filter, StringComparison.CurrentCultureIgnoreCase) || s.Note.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();

                        // filter on everything
                        return sheets.Where(s => s.Title.Contains(filter, StringComparison.CurrentCultureIgnoreCase) || s.Note.Contains(filter, StringComparison.CurrentCultureIgnoreCase) || s.Login.Contains(filter, StringComparison.CurrentCultureIgnoreCase) || s.Password.Contains(filter, StringComparison.CurrentCultureIgnoreCase) || s.UrlOrName.Contains(filter, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    }
                }
                finally
                {
                    InfoSheet.Loading = false;
                }
            }
        }


        public List<InfoSheet> GetAllSheets()
        {
            lock (locker)
            {
                InfoSheet.Loading = true;
                try
                {
                    using (var db = openConnexion())
                    {
                        var sheets = db.Table<InfoSheet>().OrderBy(sheet => sheet.Id).Select(s =>
                                                                                             {
                                                                                                 var x = s.ToUnencryptedSheet();
                                                                                                 x.IsModified = false;
                                                                                                 return x;
                                                                                             });
                        return sheets.ToList();
                    }
                }
                finally
                {
                    InfoSheet.Loading = false;
                }
            }

        }

        public InfoSheet GetInfoSheet(int id)
        {
            lock (locker)
            {
                InfoSheet.Loading = true;
                try
                {
                    using (var db = openConnexion())
                    {
                        var item = db.Table<InfoSheet>().FirstOrDefault(x => x.Id == id);
                        if (item == null) return null;
                        var realOne = item.ToUnencryptedSheet();
                        realOne.IsModified = false;
                        return realOne;
                    }
                }
                finally
                {
                    InfoSheet.Loading = false;
                }
            }
        }

        public int SaveInfoSheet(InfoSheet info)
        {
            lock (locker)
            {
                if (info == null) return -1;
                var dbSheet = info.ToCryptedSheet();
                using (var db = openConnexion())
                {
                    if (info.Id > 0)
                    {
                        db.Update(dbSheet);
                        info.IsModified = false;
                        return info.Id;
                    }
                    var i = db.Insert(dbSheet);
                    info.Id = dbSheet.Id;
                    info.IsModified = false;
                    return i;
                }
            }
        }

        public int DeleteinfoSheet(InfoSheet info)
        {
            if (info == null) return -1;
            lock (locker)
                using (var db = openConnexion())
                {
                    return db.Delete<InfoSheet>(info.Id);
                }
        }


        [LocalizationRequired(false)]
        public Tuple<int, int> GetInfoSheetCount(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter)
        {
            lock (locker)
            {
                using (var db = openConnexion())
                {
                    var all = db.ExecuteScalar<int>("SELECT COUNT(*) FROM [InfoSheet]");
                    var filtered = db.ExecuteScalar<int>(getQuery(category, proFilter, true));
                    return new Tuple<int, int>(all, filtered);
                }
            }
        }


        #endregion

        #region App parameters

        public AppSettingStorageItem GetAppSetting(int key)
        {
            lock (locker)
            {
                using (var db = openConnexion())
                {
                    var item = db.Table<AppSettingStorageItem>().FirstOrDefault(x => x.Key == key);
                    return item;
                }
            }
        }

        public int SaveAppSetting(AppSettingStorageItem setting)
        {
            lock (locker)
            {
                if (setting == null) return -1;
                using (var db = openConnexion())
                {
                    var item = db.Table<AppSettingStorageItem>().FirstOrDefault(x => x.Key == setting.Key);
                    if (item != null)
                    {
                        db.Update(setting);
                        return setting.Key;
                    }
                    return db.Insert(setting);
                }
            }
        }

        public int DeleteAppSetting(AppSettingStorageItem setting)
        {
            if (setting == null) return -1;
            lock (locker)
                using (var db = openConnexion())
                {
                    return db.Delete<AppSettingStorageItem>(setting.Key);
                }
        }

        public void DeleteAllAppSettings()
        {
            lock (locker)
                using (var db = openConnexion())
                {
                    db.DeleteAll<AppSettingStorageItem>();
                }
        }

        #endregion

        #region utils

        public string GetDababasePath()
        {
            using (var db = openConnexion())
            {
                return db.DatabasePath;
            }
        }

        #endregion

        #region Import

        public Tuple<int, int, int> ImportData(List<InfoSheet> sheets, bool skipExistingId, bool skipExistingTitle, bool clearDb)
        {
            using (var db = openConnexion())
            {
                db.BeginTransaction();
                try
                {
                    if (clearDb) db.Table<InfoSheet>().Delete(sheet => true);
                    var cpt = 0;
                    var skippedId = 0;
                    var skippedTitle = 0;
                    foreach (var sheet in sheets)
                    {
                        var idExist = db.Table<InfoSheet>().Any(x => x.Id == sheet.Id);
                        if (skipExistingId && sheet.Id > 0 && idExist)
                        {
                            skippedId++;
                            continue;
                        }
                        if (skipExistingTitle && !string.IsNullOrWhiteSpace(sheet.Title) && db.Table<InfoSheet>().Any(x => string.CompareOrdinal(x.Title, sheet.Title) == 0))
                        {
                            skippedTitle++;
                            continue;
                        }

                        var dbsheet = sheet.ToCryptedSheet();

                        if (idExist) db.Update(dbsheet);
                        else
                        {
                            var i = db.Insert(dbsheet);
                            sheet.Id = i;
                        }

                        cpt++;
                    }

                    if (clearDb && (cpt - skippedTitle - skippedId < 1)) db.Rollback();
                    else db.Commit();
                    return new Tuple<int, int, int>(cpt, skippedId, skippedTitle);
                }
                catch (Exception)
                {
                    db.Rollback();
                    return new Tuple<int, int, int>(-1, -1, -1);
                }
            }
        }

        #endregion
    }
}