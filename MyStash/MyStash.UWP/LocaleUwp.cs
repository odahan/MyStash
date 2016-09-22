using System.Globalization;
using Windows.Globalization;
using Windows.Globalization.DateTimeFormatting;
using MyStash.Helpers;
using MyStash.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocaleUwp))]
namespace MyStash.UWP
{
    public class LocaleUwp : ILocale
    {
        /// <remarks>
        /// Not sure if we can cache this info rather than querying every time
        /// </remarks>
        public string GetCurrent()
        {
            var cultureName = new DateTimeFormatter("longdate", new[] {"US"}).ResolvedLanguage;
            var lang = new CultureInfo(cultureName);
            return lang.Name;
        }


        public void SetLocale(string culture="")
        {
            if (string.IsNullOrWhiteSpace(culture))
            ApplicationLanguages.PrimaryLanguageOverride = GetCurrent();
            else ApplicationLanguages.PrimaryLanguageOverride = culture;
        }
    }
}
