using System;
using JetBrains.Annotations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyStash.Helpers
{
    [LocalizationRequired(false)]
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return Source == null ? null : ImageSource.FromResource(Source);
        }
    }
}
