using System;
using MyStash.Helpers;
using Windows.UI.Xaml;

[assembly: Xamarin.Forms.Dependency(typeof(MyStash.UWP.Timer))]
namespace MyStash.UWP
{
    public sealed class Timer : DispatcherTimer, ITimer
    {
        public event EventHandler Elapsed;

        public bool AutoReset { get; set; }


        public Timer()
        {
            Tick += baseElapsed;
        }

        public TimeSpan IntervalTime
        {
            get { return Interval; }
            set { Interval = value; }
        }

        public void Reset()
        {
            Stop();
            Start();
        }

        private void baseElapsed(object sender, object e)
        {
            if (!AutoReset) Stop();
            Elapsed?.Invoke(this, new EventArgs());
        }
    }
}
