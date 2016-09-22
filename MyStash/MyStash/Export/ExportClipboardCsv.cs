using System;
using System.IO;
using MyStash.ResX;
using Plugin.Share;

namespace MyStash.Export
{
    //*******************************************************************
    //  EXPORTCLIPBOARD - Clipboard EXPORT CLASS 
    //*******************************************************************

    /// <summary>
    /// Export data to clipboard using text format.
    /// This engine is a wrapper of the CSV engine.
    /// It uses a memory stream instead of a file stream then all data are processed in memory.
    /// </summary>
    public class ExportClipboardCsv<T> : ExportCsv<T>
    {
        /// <summary>
        /// Memory stream used to produce the output.
        /// </summary>
        private MemoryStream ms;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportClipboardCsv{T}"/> class.
        /// </summary>
        public ExportClipboardCsv()
        {
            DefaultExtension = string.Empty; // no file is created
            ShellExecute = false;
            FormatName = "Csv";
        }

        /// <summary>
        /// Overridden method executing the output. Throws an exception since this engine doesn't create any disk file that can be executed.
        /// </summary>
        protected override void ExecuteOutput()
        {
            throw new EnaxosExportException(AppResources.ExportClipboardCsv_ExecuteOutput_no_shell_execute_in_PCL_version);
        }

        /// <summary>
        /// Creates the output stream.
        /// Here the stream is a memory stream, not a file stream.
        /// </summary>
        protected override void CreateOutput()
        {
            var encoder = GetEncoder();
            ms = new MemoryStream();
            OutputStream = encoder != null ? new StreamWriter(ms, encoder) : new StreamWriter(ms);
        }

        /// <summary>
        /// Checks the file name, file extension, tests overwrite permission...
        /// If child class doesn't need any file, it must override this method.
        /// </summary>
        protected override void CheckFileName()
        {
            // do nothing and do not call inherited method
        }

        /// <summary>
        /// CloseOutput is called by Execute, it flushes and closes the output file
        /// here, it overrides base class to put the stream into the clipboard.
        /// </summary>
        protected override void CloseOutput()
        {
            ms.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(ms);
            var s = sr.ReadToEnd();
            OutputStream?.Dispose();
            OutputStream = null;
            ShellExecute = false;
            switch (Destination)
            {
                default:
                case Destination.NativeExporter:
                case Destination.Clipboard:
                    CrossShare.Current.SetClipboardText(s, AppResources.ExportClipboardCsv_CloseOutput_MyStash_CSV_data);
                    break;
                case Destination.Share:
                    CrossShare.Current.Share(s, AppResources.ExportClipboardCsv_CloseOutput_MyStash_CSV_data);
                    break;
            }
        }
    }
}
