using System;

namespace MyStash.Helpers
{
    public interface ITimer
    {
        event EventHandler Elapsed;

        bool AutoReset { get; set; }
        TimeSpan IntervalTime { get; set; }

        void Start();
        void Stop();
        void Reset();


    }
   
}
