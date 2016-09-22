using System;

namespace MyStash.Export
{

    /// <summary>
    /// Progress events
    /// </summary>
    public class ExportProgressEventArgs : EventArgs
    {
        public ExportProgressEventArgs()
        {
            Count = 0;
            Current = 0;
            EndProcessRequested = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProgressEventArgs"/> class.
        /// </summary>
        /// <param name="count">Data count.</param>
        /// <param name="current">Current index</param>
        /// <param name="endProcessRequested">if set to <c>true</c> end of process is requested.</param>
        public ExportProgressEventArgs(int count, int current, bool endProcessRequested)
        {
            Count = count;
            Current = current;
            EndProcessRequested = endProcessRequested;
        }

        /// <summary>
        /// Gets or sets the count of data to export
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the index of currently exported data
        /// </summary>
        /// <value>The current.</value>
        public int Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether end of process is requested.
        /// </summary>
        /// <value><c>true</c> if [end process]; otherwise, <c>false</c>.</value>
        public bool EndProcessRequested { get; set; }
    }
}
