using System;
using System.Globalization;
using Xamarin.Forms;
using System.Threading;
using MyStash.Droid;
using MyStash.Helpers;

[assembly:Dependency(typeof(LocaleDroid))]

namespace MyStash.Droid
{
    public class LocaleDroid : ILocale
    {
        public void SetLocale(string culture ="")
        {
            CultureInfo ci;
            if (string.IsNullOrWhiteSpace(culture))
            {
                var androidLocale = Java.Util.Locale.Default; // user's preferred locale
                var netLocale = androidLocale.ToString().Replace("_", "-");
                ci = new System.Globalization.CultureInfo(netLocale);
            }
            else
            {
                try
                {
                    ci = new CultureInfo(culture);
                }
                catch
                {
                    ci= new CultureInfo(GetCurrent());
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
            var androidLocale = Java.Util.Locale.Default; // user's preferred locale

            // en, es, ja
            var netLanguage = androidLocale.Language.Replace("_", "-");
            // en-US, es-ES, ja-JP
            var netLocale = androidLocale.ToString().Replace("_", "-");

            #region Debugging output
            Console.WriteLine("android:  " + androidLocale);
            Console.WriteLine("netlang:  " + netLanguage);
            Console.WriteLine("netlocale:" + netLocale);

            var ci = new System.Globalization.CultureInfo(netLocale);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            Console.WriteLine("thread:  " + Thread.CurrentThread.CurrentCulture);
            Console.WriteLine("threadui:" + Thread.CurrentThread.CurrentUICulture);
            #endregion

            return netLocale;
        }
    }
}