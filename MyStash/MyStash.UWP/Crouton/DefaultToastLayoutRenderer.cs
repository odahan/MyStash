using System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MyStash.Crouton;

namespace MyStash.UWP.Crouton
{
    public sealed class DefaultToastLayoutRenderer : IToastLayoutCustomRenderer
    {
        private readonly Func<object, BitmapImage> iconForCustomTypeResolver;
        private readonly Func<object, Brush> backgroundForCustomTypeResolver;

        /// <param name="iconForCustomTypeResolver">resolves a bitmapImage for ToastNotificationType.Custom type. object - is a context passed to Notify</param>
        /// <param name="backgroundForCustomTypeResolver">resolves a background brush for ToastNotificationType.Custom type. object - is a context passed to Notify</param>
        public DefaultToastLayoutRenderer(Func<object, BitmapImage> iconForCustomTypeResolver = null, Func<object, Brush> backgroundForCustomTypeResolver = null)
        {
            this.iconForCustomTypeResolver = iconForCustomTypeResolver;
            this.backgroundForCustomTypeResolver = backgroundForCustomTypeResolver;
        }

        public UIElement Render(ToastNotificationType type, string title, string description, object context, out Brush brush)
        {
            BitmapImage iconImage;

            switch (type)
            {
                case ToastNotificationType.Success:
                    brush = new SolidColorBrush(Color.FromArgb(255, 69, 145, 34));
                    iconImage = loadBitmapImage("success.png");
                    break;
                case ToastNotificationType.Warning:
                    brush = new SolidColorBrush(Color.FromArgb(255, 180, 125, 1));
                    iconImage = loadBitmapImage("warning.png");
                    break;
                case ToastNotificationType.Error:
                    brush = new SolidColorBrush(Color.FromArgb(255, 206, 24, 24));
                    iconImage = loadBitmapImage("error.png");
                    break;
                case ToastNotificationType.Info: //info by default
                    brush = new SolidColorBrush(Color.FromArgb(255, 42, 112, 153));
                    iconImage = loadBitmapImage("info.png");
                    break;
                case ToastNotificationType.Custom:
                    brush = backgroundForCustomTypeResolver(context);
                    iconImage = iconForCustomTypeResolver(context);
                    break;
                default:
                    throw new ArgumentException(type.ToString());
            }

            //this actually could be done in XAML, but I decided not to waste performance on XAML parsing
            //since UI is simple and if you want - you can override it by implementing IToastLayoutCustomRenderer

            var titleTb = new TextBlock
            {
                Text = title,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 6, 0, 0),
                TextTrimming = TextTrimming.None,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 22,
                FontWeight = FontWeights.Bold
            };
            Grid.SetColumn(titleTb, 1);
            Grid.SetRow(titleTb, 0);

            var descTb = new TextBlock
            {
                Text = description,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, -4, 0, 0),
                TextTrimming = TextTrimming.WordEllipsis,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 18
            };
            Grid.SetColumn(descTb, 1);
            Grid.SetRow(descTb, 1);

            var layout = new Grid();
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            layout.ColumnDefinitions.Add(new ColumnDefinition());
            layout.RowDefinitions.Add(new RowDefinition());
            layout.RowDefinitions.Add(new RowDefinition());

            layout.Children.Add(titleTb);
            layout.Children.Add(descTb);

            if (iconImage != null) //do not add image if iconImage is empty
            {
                var image = new Image
                {
                    Width = 42,
                    Height = 42,
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Source = iconImage
                };
                Grid.SetRowSpan(image, 2);
                layout.Children.Add(image);
            }
            
            return layout;
        }

        public bool IsTappable => true;

        public bool HasCloseButton => true;

        private static BitmapImage loadBitmapImage(string fileName)
        {
            var uri = new Uri("ms-appx://MyStash.UWP/Crouton/Icons/" + fileName, UriKind.Absolute);
            var bitmap = new BitmapImage(uri);
            return bitmap;

        }
    }
}
