using System;
using System.Collections.Generic;
using MyStash.Models;

namespace MyStash.Service
{
    public interface IDataProvider
    {
        #region sheets
        GroupedInfoSheetGroups GetInfoSheetList(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter, string filter = null);
        InfoSheet GetInfoSheet(int id);
        int SaveInfoSheet(InfoSheet info);
        int DeleteinfoSheet(InfoSheet info);
        /// <summary>
        /// return the count of sheet.
        /// Item1 is total count of the category, Item2 is the same once the pro filter is applied
        /// </summary>
        /// <param name="category"></param>
        /// <param name="proFilter"></param>
        /// <returns></returns>
        Tuple<int, int> GetInfoSheetCount(InfoSheet.CategoryFilter category, InfoSheet.ProFilter proFilter);
        List<InfoSheet> GetAllSheets();

            #endregion

        #region App settings

        AppSettingStorageItem GetAppSetting(int key);
        int SaveAppSetting(AppSettingStorageItem setting);
        int DeleteAppSetting(AppSettingStorageItem setting);
        void DeleteAllAppSettings();
        #endregion

        #region utils
        string GetDababasePath();
        #endregion

        #region Import

        Tuple<int, int, int> ImportData(List<InfoSheet> sheets, bool skipExistingId, bool skipExistingTitle, bool clearDb);

        #endregion
    }
}
