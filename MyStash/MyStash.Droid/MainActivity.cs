using Android.App;
using Android.Content.PM;
using Android.OS;
using MyStash.Droid.Crouton;
//using ImageCircle.Forms.Plugin.Droid;
using Xamarin.Forms;

namespace MyStash.Droid
{
    [Activity(Label = "MyStash", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            //ImageCircleRenderer.Init();
            DependencyService.Register<ToastNotificatorImplementation>();
            ToastNotificatorImplementation.Init(this);
            LoadApplication(new App());
        }
    }
}

