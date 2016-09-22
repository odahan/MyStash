using System;
using System.Globalization;
using Xamarin.Forms;
using System.Threading;
using Foundation;
using MyStash.Helpers;
using MyStash.iOS;

[assembly: Dependency(typeof(LocaleiOS))]

namespace MyStash.iOS
{
    public class LocaleiOS : ILocale
    {
        public void SetLocale(string culture="")
        {

            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            var netLocale = iosLocaleAuto.Replace("_", "-");
            CultureInfo ci;
            if (string.IsNullOrWhiteSpace(culture))
            {
                try
                {
                    ci = new CultureInfo(netLocale);
                }
                catch
                {
                    ci = new CultureInfo(GetCurrent());
                }
            }
            else
            {
                try
                {
                    ci=new CultureInfo(culture);
                }
                catch (Exception)
                {
                    ci = new CultureInfo(GetCurrent());
                }
            }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
        /// <remarks>
        /// Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public string GetCurrent()
        {
            var iosLocaleAuto = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
            var iosLanguageAuto = NSLocale.AutoUpdatingCurrentLocale.LanguageCode;
            var netLocale = iosLocaleAuto.Replace("_", "-");
            var netLanguage = iosLanguageAuto.Replace("_", "-");

            #region Debugging Info
            // prefer *Auto updating properties
            //			var iosLocale = NSLocale.CurrentLocale.LocaleIdentifier;
            //			var iosLanguage = NSLocale.CurrentLocale.LanguageCode;
            //			var netLocale = iosLocale.Replace ("_", "-");
            //			var netLanguage = iosLanguage.Replace ("_", "-");

            Console.WriteLine("nslocaleid:" + iosLocaleAuto);
            Console.WriteLine("nslanguage:" + iosLanguageAuto);
            Console.WriteLine("ios:" + iosLanguageAuto + " " + iosLocaleAuto);
            Console.WriteLine("net:" + netLanguage + " " + netLocale);

            System.Globalization.CultureInfo ci;
            try
            {
                ci = new System.Globalization.CultureInfo(netLocale);
            }
            catch
            {
                ci = new System.Globalization.CultureInfo(NSLocale.PreferredLanguages[0]);
            }
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Console.WriteLine("thread:  " + Thread.CurrentThread.CurrentCulture);
            Console.WriteLine("threadui:" + Thread.CurrentThread.CurrentUICulture);
            #endregion

            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-");
                Console.WriteLine("preferred:" + netLanguage);
            }
            else
            {
                netLanguage = "en"; // default, shouldn't really happen :)
            }
            return netLanguage;
        }
    }
}