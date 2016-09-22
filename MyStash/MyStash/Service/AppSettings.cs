using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Microsoft.Practices.ServiceLocation;
using MyStash.Helpers;
using MyStash.ResX;
using SQLite.Net.Attributes;

namespace MyStash.Service
{

    public enum SupportedCulture { Default=0, English=1, French=2, Italian=3, German=4, Russian=5}


    // App Settings are using Roaming folder to store key files.
    public interface IAppSettings
    {
        /// <summary>
        /// Display lines in list (hz item separator)
        /// </summary>
        bool DisplayLinesInLists { get; set; }
        /// <summary>
        /// Search through notes as well
        /// </summary>
        bool SearchInNote { get; set; }
        /// <summary>
        /// Search in all string fields
        /// </summary>
        bool SearchEverywhere { get; set; }
        /// <summary>
        /// Show/Hide item date in list
        /// </summary>
        bool DisplayDateInLists { get; set; }

        /// <summary>
        /// Lock the app when it is hidden
        /// </summary>
        bool LockOnHide { get; set; }

        /// <summary>
        /// Automatic time out
        /// </summary>
        bool AutoTimeOut { get; set; }

        /// <summary>
        /// Time out delay in seconds
        /// </summary>
        int TimeOutSeconds { get; set; }

        /// <summary>
        /// login digital keyboard key shuffle on/off
        /// </summary>
        bool AreDigitsShuffled { get; set; }

        /// <summary>
        /// Preferred culture, override auto detection
        /// Only available for supported languages (mostly fr, us)
        /// </summary>
        SupportedCulture PreferredCulture { get; set; }

        /// <summary>
        /// Get the main password
        /// </summary>
        /// <returns></returns>
        string GetDoorLock();

        /// <summary>
        /// Set the main password
        /// </summary>
        /// <param name="doorLock"></param>
        void SetDoorLock(string doorLock);


        SheetCrypting Algorithm { get; set; }

        /// <summary>
        /// Supported culture dictionary (name)
        /// </summary>
        Dictionary<SupportedCulture, string> CultureDictionary { get; }
        /// <summary>
        /// Supported culture dictionary (code)
        /// </summary>
        Dictionary<SupportedCulture, string> CultureCodeDictionary { get; }
    }

    // Key list
    public enum SettingKeys
    {
        DisplayLinesInLists = 1,
        SearchInNote = 2,
        SearchEverywhere = 3,
        DisplayDateInLists = 4,
        DoorLock = 5,
        LockOnHide = 6,
        AutoTimeOut = 7,
        TimeOutSeconds = 8,
        ShuffleKeyboard =9,
        PrefferedCulture = 10,
        Algorithm = 11
    }

    public class AppSettingStorageItem
    {
        [PrimaryKey] // not autoinc, key is given by the app
        public int Key { get; set; }
        public string Value { get; set; }
    }


    public class AppSettings : ObservableObject, IAppSettings
    {

        public Dictionary<SupportedCulture, string> CultureDictionary { get; } = new Dictionary<SupportedCulture, string>
                                                                             {
                                                                                 {SupportedCulture.Default, AppResources.AppSettings_CultureDictionary_Default},
                                                                                 {SupportedCulture.English, AppResources.AppSettings_CultureDictionary_English},
                                                                                 {SupportedCulture.French, AppResources.AppSettings_CultureDictionary_French},
                                                                                 {SupportedCulture.German, AppResources.AppSettings_CultureDictionary_German},
                                                                                 {SupportedCulture.Italian, AppResources.AppSettings_CultureDictionary_Italian},
                                                                                 {SupportedCulture.Russian, AppResources.AppSettings_CultureDictionary_Russian}
                                                                             };

        [LocalizationRequired(false)]
        public Dictionary<SupportedCulture, string> CultureCodeDictionary { get; } = new Dictionary<SupportedCulture, string>
                                                                             {
                                                                                 {SupportedCulture.Default, ""},
                                                                                 {SupportedCulture.English, "en"},
                                                                                 {SupportedCulture.French, "fr"},
                                                                                 {SupportedCulture.German, "de"},
                                                                                 {SupportedCulture.Italian, "it"},
                                                                                 {SupportedCulture.Russian, "ru"}
                                                                             };


        // privata and cache dict
        private readonly Dictionary<SettingKeys, object> cache = new Dictionary<SettingKeys, object>();
        // db access
        private readonly IDataProvider db;

        // Ctor
        public AppSettings()
        {
            db = ServiceLocator.Current.GetInstance<IDataProvider>();
            loadValues();
        }

        #region utils
        private string getStringSetting(SettingKeys key, string defaultValue)
        {
            var item = db.GetAppSetting((int)key);
            if (item == null) return defaultValue;
            return item.Value;
        }

        private void setStringSetting(SettingKeys key, string value)
        {
            var item = new AppSettingStorageItem { Key = (int)key, Value = value };
            db.SaveAppSetting(item);
        }

        private bool getBoolSetting(SettingKeys key, bool defaultValue)
        {
            var item = db.GetAppSetting((int)key);
            if (item == null) return defaultValue;
            return bool.Parse(item.Value);
        }

        private void setBoolSetting(SettingKeys key, bool value)
        {
            var item = new AppSettingStorageItem { Key = (int)key, Value = value.ToString() };
            db.SaveAppSetting(item);
        }

        private int getIntSetting(SettingKeys key, int defaultValue)
        {
            var item = db.GetAppSetting((int)key);
            if (item == null) return defaultValue;
            return int.Parse(item.Value);
        }

        private void setIntSetting(SettingKeys key, int value)
        {
            var item = new AppSettingStorageItem { Key = (int)key, Value = value.ToString() };
            db.SaveAppSetting(item);
        }
        #endregion

        #region Load/Save
        // Read from db all values and fill cache + inpc
        private void loadValues()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            cache[SettingKeys.DisplayLinesInLists] = getBoolSetting(SettingKeys.DisplayLinesInLists, true);
            RaisePropertyChanged(nameof(DisplayLinesInLists));


            cache[SettingKeys.SearchInNote] = getBoolSetting(SettingKeys.SearchInNote, false);
            RaisePropertyChanged(nameof(SearchInNote));


            cache[SettingKeys.SearchEverywhere] = getBoolSetting(SettingKeys.SearchEverywhere, false);
            RaisePropertyChanged(nameof(SearchEverywhere));

            cache[SettingKeys.DisplayDateInLists] = getBoolSetting(SettingKeys.DisplayDateInLists, true);
            RaisePropertyChanged(nameof(DisplayDateInLists));

            cache[SettingKeys.LockOnHide] = getBoolSetting(SettingKeys.LockOnHide, true);
            RaisePropertyChanged(nameof(LockOnHide));

            cache[SettingKeys.AutoTimeOut] = getBoolSetting(SettingKeys.AutoTimeOut, true);
            RaisePropertyChanged(nameof(AutoTimeOut));

            cache[SettingKeys.TimeOutSeconds] = getIntSetting(SettingKeys.TimeOutSeconds, 120);
            RaisePropertyChanged(nameof(TimeOutSeconds));

            cache[SettingKeys.ShuffleKeyboard] = getBoolSetting(SettingKeys.ShuffleKeyboard, true);
            RaisePropertyChanged(nameof(AreDigitsShuffled));

            cache[SettingKeys.PrefferedCulture] = (SupportedCulture)getIntSetting(SettingKeys.PrefferedCulture, 0);
            RaisePropertyChanged(nameof(PreferredCulture));

            cache[SettingKeys.Algorithm] = (SheetCrypting)getIntSetting(SettingKeys.Algorithm, (int)SheetCrypting.Algo1);
            RaisePropertyChanged(nameof(Algorithm));

            // ReSharper restore ExplicitCallerInfoArgument
            Messenger.Default.Send(new NotificationMessage(Utils.GlobalMessages.SettingsChanged.ToString()));
        }

        // Write all values to disk
        private void updateValues()
        {
            setBoolSetting(SettingKeys.DisplayLinesInLists, (bool)cache[SettingKeys.DisplayLinesInLists]);
            setBoolSetting(SettingKeys.SearchInNote, (bool)cache[SettingKeys.SearchInNote]);
            setBoolSetting(SettingKeys.SearchEverywhere, (bool)cache[SettingKeys.SearchEverywhere]);
            setBoolSetting(SettingKeys.DisplayDateInLists, (bool)cache[SettingKeys.DisplayDateInLists]);
            setBoolSetting(SettingKeys.LockOnHide, (bool)cache[SettingKeys.LockOnHide]);
            setBoolSetting(SettingKeys.AutoTimeOut, (bool)cache[SettingKeys.AutoTimeOut]);
            setIntSetting(SettingKeys.TimeOutSeconds, (int)cache[SettingKeys.TimeOutSeconds]);
            setBoolSetting(SettingKeys.ShuffleKeyboard, (bool)cache[SettingKeys.ShuffleKeyboard]);
            setIntSetting(SettingKeys.PrefferedCulture, (int)cache[SettingKeys.PrefferedCulture]);
            setIntSetting(SettingKeys.Algorithm, (int)cache[SettingKeys.Algorithm]);
            Messenger.Default.Send(new NotificationMessage(Utils.GlobalMessages.SettingsChanged.ToString()));
        }

        #endregion

        // *** settings ***

        /// <summary>
        /// Display lines in list (hz item separator)
        /// </summary>
        public bool DisplayLinesInLists
        {
            get { return (bool)cache[SettingKeys.DisplayLinesInLists]; }
            set
            {
                cache[SettingKeys.DisplayLinesInLists] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool SearchInNote
        {
            get { return (bool)cache[SettingKeys.SearchInNote]; }
            set
            {
                cache[SettingKeys.SearchInNote] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool SearchEverywhere
        {
            get { return (bool)cache[SettingKeys.SearchEverywhere]; }
            set
            {
                cache[SettingKeys.SearchEverywhere] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool DisplayDateInLists
        {
            get { return (bool)cache[SettingKeys.DisplayDateInLists]; }
            set
            {
                cache[SettingKeys.DisplayDateInLists] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool LockOnHide
        {
            get { return (bool)cache[SettingKeys.LockOnHide]; }
            set
            {
                cache[SettingKeys.LockOnHide] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool AutoTimeOut
        {
            get { return (bool) cache[SettingKeys.AutoTimeOut]; }
            set
            {
                cache[SettingKeys.AutoTimeOut] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }


        public int TimeOutSeconds
        {
            get { return (int) cache[SettingKeys.TimeOutSeconds]; }
            set
            {
                cache[SettingKeys.TimeOutSeconds] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public bool AreDigitsShuffled
        {
            get { return (bool) cache[SettingKeys.ShuffleKeyboard]; }
            set
            {
                cache[SettingKeys.ShuffleKeyboard] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public SupportedCulture PreferredCulture
        {
            get { return (SupportedCulture) cache[SettingKeys.PrefferedCulture]; }
            set
            {
                cache[SettingKeys.PrefferedCulture] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public SheetCrypting Algorithm
        {
            get { return (SheetCrypting) cache[SettingKeys.Algorithm]; }
            set
            {
                cache[SettingKeys.Algorithm] = value;
                updateValues();
                RaisePropertyChanged();
            }
        }

        public string GetDoorLock()
        {
            if (cache.ContainsKey(SettingKeys.DoorLock)
                && !string.IsNullOrWhiteSpace(cache[SettingKeys.DoorLock] as string))
            {
                return Encryption.Instance.Decrypt((string)cache[SettingKeys.DoorLock],SheetCrypting.Algo1);
            }
            var data = getStringSetting(SettingKeys.DoorLock, null);
            if (string.IsNullOrWhiteSpace(data))
            {
                if (cache.ContainsKey(SettingKeys.DoorLock)) cache.Remove(SettingKeys.DoorLock);
                return null;
            }
            cache[SettingKeys.DoorLock] = data;
            return Encryption.Instance.Decrypt(data,SheetCrypting.Algo1);
        }

        public void SetDoorLock(string doorLock)
        {
            if (string.IsNullOrWhiteSpace(doorLock))
            {
                db.DeleteAppSetting(new AppSettingStorageItem { Key = (int)SettingKeys.DoorLock });
                if (cache.ContainsKey(SettingKeys.DoorLock)) cache.Remove(SettingKeys.DoorLock);
                return;
            }
            cache[SettingKeys.DoorLock] = Encryption.Instance.Encrypt(doorLock,SheetCrypting.Algo1);
            setStringSetting(SettingKeys.DoorLock, cache[SettingKeys.DoorLock] as string);
        }



    }
}
