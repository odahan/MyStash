using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using MyStash.Crouton;

namespace MyStash.UWP.Crouton
{
    public interface IToastLayoutCustomRenderer
    {
        UIElement Render(ToastNotificationType type, string title, string description, object context, out Brush backgroundBrush);

        bool IsTappable { get; }

        bool HasCloseButton { get; }
    }
}
