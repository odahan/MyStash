using System;

namespace MyStash.Export
{
    /// <summary>
    /// Exception class for export engines
    /// </summary>
    public class EnaxosExportException : Exception
    {
        /// <summary>
        /// Exception constructor.
        /// </summary>
        /// <param name="message">Error message</param>
        public EnaxosExportException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Exception constructor allowing extra information.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="extraInfo">Extra information</param>
        public EnaxosExportException(string message, object extraInfo)
            : base(message)
        {
            Info = extraInfo;
        }

        /// <summary>
        /// Extra information if needed
        /// </summary>
        public object Info { set; get; }
    }
}
