using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Droid;
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


            AppDomain.CurrentDomain.UnhandledException += currentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += taskSchedulerOnUnobservedTaskException;
            /*
//iOS: Different than Android. Must be in FinishedLaunching, not in Main.
// In AppDelegate
public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary options)
{
    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;  
    ...
}  
 */


            global::Xamarin.Forms.Forms.Init(this, bundle);

            displayCrashReport();

            CachedImageRenderer.Init();
            DependencyService.Register<ToastNotificatorImplementation>();
            ToastNotificatorImplementation.Init(this);
            LoadApplication(new App());
        }


        #region Error handling

        private static void taskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            logUnhandledException(newExc);
        }

        private static void currentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            logUnhandledException(newExc);
        }

        private static void logUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = $"Time: {DateTime.Now}\r\nError: Unhandled Exception\r\n{exception}";
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        [Conditional("DEBUG")]
        private void displayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this)
                .SetPositiveButton("Clear", (sender, args) =>
                {
                    File.Delete(errorFilePath);
                })
                .SetNegativeButton("Close", (sender, args) =>
                {
                    // User pressed Close.
                })
                .SetMessage(errorText)
                .SetTitle("Crash Report")
                .Show();
        }

        #endregion

    }
}

