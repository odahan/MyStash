using Android.App;
using Android.Views;
using MyStash.Crouton;

namespace MyStash.Droid.Crouton
{
    public interface IToastLayoutCustomRenderer
    {
        View Render(Activity activity, ToastNotificationType type, string title, string description, object context);
    }
}
