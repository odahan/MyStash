using System.Globalization;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;
using MyStash.ResX;
using Xamarin.Forms;

namespace MyStash.Helpers
{
    public static class Translator
    {
        public static void SetLocale(string culture="")
        {
            var netlanguage = DependencyService.Get<ILocale>().GetCurrent();
            DependencyService.Get<ILocale>().SetLocale(culture);
            localeCache = string.IsNullOrWhiteSpace(culture) ? netlanguage : culture;
            AppResources.Culture = new CultureInfo(localeCache);
            
        }

        private static string localeCache;

        public static string Locale()
        {
            return string.IsNullOrWhiteSpace(localeCache) ? localeCache = DependencyService.Get<ILocale>().GetCurrent() : localeCache;
        }

        [LocalizationRequired(false)]
        public static string Localize(string key)
        {
            var netLanguage = Locale();
            // Platform-specific
            var temp = new ResourceManager("MyStash.Resx.AppResources", typeof(Translator).GetTypeInfo().Assembly);
            return temp.GetString(key, new CultureInfo(netLanguage));
        }
    }
}
