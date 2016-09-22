using System;
using System.Threading;
using System.Threading.Tasks;
using MyStash.Crouton;

namespace MyStash.iOS.Crouton
{
    public class ToastNotificatorImplementation : IToastNotificator
    {
        private static MessageBarStyleSheet customStyle;

        public Task<bool> Notify(ToastNotificationType type, string title, string description, TimeSpan duration, object context, bool clickable = true)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            MessageBarManager.SharedInstance.ShowMessage(title, description, type, b =>
                {
                    if (clickable)
                    {
                        taskCompletionSource.TrySetResult(b);
                    }
                }, duration, customStyle);
            return taskCompletionSource.Task;
        }

        public Task NotifySticky(ToastNotificationType type, string title, string description, object context = null,
            bool clickable = true, CancellationToken cancellationToken = new CancellationToken(), bool modal = false)
        {
            throw new NotImplementedException("yet");
        }

        public void HideAll()
        {
            MessageBarManager.SharedInstance.HideAll();
        }

        public static void Init(MessageBarStyleSheet customStyle = null)
        {
            ToastNotificatorImplementation.customStyle = customStyle ?? new MessageBarStyleSheet();
        }
    }
}