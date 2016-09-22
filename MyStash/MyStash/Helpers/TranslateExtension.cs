using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;
using System.Diagnostics;

namespace MyStash.Helpers
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null) return null;
            var translated = Translator.Localize(Text);

            return translated;
        }
    }
}
