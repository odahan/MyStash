using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using MyStash.Crouton;

namespace MyStash.Droid.Crouton
{
    public class ToastNotificatorImplementation : IToastNotificator
    {
        private static IToastLayoutCustomRenderer customRenderer;

        private static Activity activity;

        public Task<bool> Notify(ToastNotificationType type, string title, string description, TimeSpan duration, object context = null, bool clickable = true)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            
            if (activity == null)
                return Task.FromResult(false);

            var view = customRenderer.Render(activity, type, title, description, context);

            var crouton = new Crouton(activity, view, (int)duration.TotalMilliseconds, 
                b =>
                {
                    if (clickable)
                    {
                        taskCompletionSource.TrySetResult(b);
                    }
                }, context);
        
            crouton.Show();
            return taskCompletionSource.Task;
        }
        
        public Task NotifySticky(ToastNotificationType type, string title, string description, object context = null,
            bool clickable = true, CancellationToken cancellationToken = new CancellationToken(), bool modal = false)
        {
            throw new NotImplementedException("yet");
        }

        public void HideAll()
        {
            Manager.Instance.RemoveCroutons();
        }
               
        /// <summary>
        /// You can pass your custom renderer for toast layout, in case of null DefaultToastLayoutRenderer will be used
        /// </summary>
        /// <param name="act">The current act. In Xamarin Forms pass the instance of the MainActity e.g. Init(this);</param>
        /// <param name="custRenderer"></param>
        public static void Init(Activity act, IToastLayoutCustomRenderer custRenderer = null)
        {
            activity = act;
            customRenderer = custRenderer ?? new DefaultToastLayoutRenderer();
        }
    }
}