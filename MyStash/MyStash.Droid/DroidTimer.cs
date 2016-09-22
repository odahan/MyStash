using System;
using MyStash.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(MyStash.Droid.Timer))]
namespace MyStash.Droid
{
    public sealed class Timer : System.Timers.Timer, ITimer
    {
        public new event EventHandler Elapsed;

        public Timer()
        {
            base.Elapsed += baseElapsed;
        }

        public Timer(double interval) : base(interval)
        {
            base.Elapsed += baseElapsed;
        }

        public TimeSpan IntervalTime
        {
            get { return new TimeSpan(0, 0, 0, 0, (int)Interval); }
            set { Interval = value.TotalMilliseconds; }
        }

        public void Reset()
        {
            Stop();
            Start();
        }

        private bool vDisposed;
        protected override void Dispose(bool disposing)
        {
            if (vDisposed)
            {
                return;
            }

            base.Elapsed -= baseElapsed;
            vDisposed = true;

            base.Dispose(disposing);
        }

        private void baseElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Elapsed?.Invoke(this, new EventArgs());
        }
    }

}