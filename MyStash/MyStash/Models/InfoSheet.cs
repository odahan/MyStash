using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using MyStash.Helpers;
using MyStash.ResX;
using SQLite.Net.Attributes;

namespace MyStash.Models
{



    public class InfoSheet : ObservableObject, ICloneable
    {
        private int id;
        private string title;
        private ProFilter pro;
        private string note;
        private DateTime createdOn;
        private DateTime modifiedOn;
        private string urlOrName;
        private string login;
        private string password;
        private CategoryFilter category;
        private SheetCrypting crypting = SheetCrypting.None;

        public InfoSheet()
        {
            createdOn = modifiedOn = DateTime.Now;
        }

        public enum CategoryFilter { All = 0, Login = 1, Note = 2, Misc = 3 }
        public enum ProFilter { All = 0, Profesional = 1, Personal = 2 }

        public static readonly Dictionary<ProFilter, string> ProFilterDictionary = new Dictionary<ProFilter, string>
                                                                             {
                                                                                 {ProFilter.All, AppResources.InfoSheet_ProFilterDictionary_All},
                                                                                 {ProFilter.Personal, AppResources.InfoSheet_ProFilterDictionary_Personal},
                                                                                 {ProFilter.Profesional, AppResources.InfoSheet_ProFilterDictionary_Professional}
                                                                             };

        private static List<Tuple<ProFilter, string>> profilterTuples => ProFilterDictionary.Select(pair => new Tuple<ProFilter, string>(pair.Key, pair.Value)).ToList();
        public static List<Tuple<ProFilter, string>> ProfilterTuples => profilterTuples;

        public static readonly Dictionary<CategoryFilter, string> CategoryDictionary = new Dictionary<CategoryFilter, string>
                                                                             {
                                                                                 {CategoryFilter.All, AppResources.InfoSheet_ProFilterDictionary_All},
                                                                                 {CategoryFilter.Login, AppResources.InfoSheet_CategoryDictionary_Login},
                                                                                 {CategoryFilter.Misc, AppResources.InfoSheet_CategoryDictionary_Misc},
                                                                                 {CategoryFilter.Note, AppResources.InfoSheet_CategoryDictionary_Note},
                                                                             };

        private static List<Tuple<CategoryFilter, string>> categoryTuples => CategoryDictionary.Select(pair => new Tuple<CategoryFilter, string>(pair.Key, pair.Value)).ToList();
        public static List<Tuple<CategoryFilter, string>> CategoryTuples => categoryTuples;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get { return id; }
            set { Set(ref id, value); }
        }

        [LocalizationRequired(false)]
        [Indexed(Name = "TitleIdx", Order = 1, Unique = false)]
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }


        public ProFilter Pro
        {
            get { return pro; }
            set { if (Set(ref pro, value)) RaisePropertyChanged(nameof(IsPro)); }
        }

        [Ignore]
        public bool IsPro
        {
            get { return Pro == ProFilter.Profesional; }
            set { Pro = value ? ProFilter.Profesional : ProFilter.Personal; }
        }

        [Ignore]
        public string ProStr => ProFilterDictionary[Pro];

        public string Note
        {
            get { return note; }
            set { Set(ref note, value); }
        }

        public DateTime CreatedOn
        {
            get { return createdOn; }
            set
            {
                if (Set(ref createdOn, value))
                    RaisePropertyChanged(nameof(CreatedOnStr));
            }
        }

        [LocalizationRequired(false)]
        [Ignore]
        public string CreatedOnStr => createdOn.ToString("d");

        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set
            {
                if (Set(ref modifiedOn, value))
                    RaisePropertyChanged(nameof(ModifiedOnStr));
            }
        }

        [LocalizationRequired(false)]
        [Ignore]
        public string ModifiedOnStr => modifiedOn.ToString("d");

        public string UrlOrName
        {
            get { return urlOrName; }
            set { Set(ref urlOrName, value); }
        }

        public string Login
        {
            get { return login; }
            set { Set(ref login, value); }
        }

        public string Password
        {
            get { return password; }
            set { Set(ref password, value); }
        }

        public CategoryFilter Category
        {
            get { return category; }
            set { if (value != CategoryFilter.All) Set(ref category, value); }
        }

        [Ignore]
        public string CategoryStr => CategoryDictionary[Category];

        private bool isModified;

        [Ignore]
        public bool IsModified
        {
            get { return isModified; }
            set { Set(ref isModified, value); }
        }

        [Default(value:SheetCrypting.Algo1)]
        public SheetCrypting Crypting
        {
            get { return crypting; }
            set { Set(ref crypting, value); }
        }


        private static readonly object locker = new object();
        private static bool loading;

        [Ignore]
        public static bool Loading
        {
            get { lock (locker) return loading; }
            set { lock (locker) loading = value; }
        }

        public override void RaisePropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.RaisePropertyChanged(propertyName);
            if (propertyName == nameof(ModifiedOn)) return;
            if (propertyName == nameof(ModifiedOnStr)) return;
            if (propertyName == nameof(IsModified)) return;
            if (propertyName == nameof(Crypting)) return;
            IsModified = !Loading;
            if (!Loading) ModifiedOn = DateTime.Now;
        }

        [LocalizationRequired(false)]
        public override string ToString()
        {
            return $"id {id} title {title} pro {ProStr} cat {CategoryStr} algo {Crypting}";
        }

        public object Clone()
        {
            return new InfoSheet
            {
                id = id,
                title = title,
                pro = pro,
                note = note,
                createdOn = createdOn,
                modifiedOn = modifiedOn,
                urlOrName = urlOrName,
                login = login,
                password = password,
                category = category,
                isModified = isModified,
                crypting = crypting
            };
        }

        private static string strToCrypto(string uncrypted)
        {
            return (string.IsNullOrWhiteSpace(uncrypted)) ? string.Empty : Encryption.Instance.Encrypt(uncrypted, App.Locator.AppSettings.Algorithm);
        }

        private static string cryptoToStr(string crypted, SheetCrypting algorithm)
        {
            return string.IsNullOrWhiteSpace(crypted) 
                ? string.Empty 
                : Encryption.Instance.Decrypt(crypted, algorithm);
        }

        public InfoSheet ToCryptedSheet()
        {
            var clone = (InfoSheet)Clone();
            if (Crypting != SheetCrypting.None) // already crypted
            {
                // same algorithm than settings
                if (Crypting == App.Locator.AppSettings.Algorithm)
                    return clone;
                // algorithm is not the same, to update the encryption we decrypt and let it be crypted with current algorithm
                clone = clone.ToUnencryptedSheet();
            }
            clone.login = strToCrypto(clone.login);
            clone.note = strToCrypto(clone.note);
            clone.password = strToCrypto(clone.password);
            clone.urlOrName = strToCrypto(clone.urlOrName);
            clone.crypting = App.Locator.AppSettings.Algorithm;
            return clone;
        }

        public InfoSheet ToUnencryptedSheet()
        {
            var clone = (InfoSheet)Clone();
            if (Crypting == SheetCrypting.None) return clone;
            clone.login = cryptoToStr(clone.login, Crypting);
            clone.note = cryptoToStr(clone.note, Crypting);
            clone.password = cryptoToStr(clone.password, Crypting);
            clone.urlOrName = cryptoToStr(clone.urlOrName, Crypting);
            clone.crypting = SheetCrypting.None;
            return clone;
        }
    }
}
