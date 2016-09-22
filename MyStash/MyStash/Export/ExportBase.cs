using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GalaSoft.MvvmLight;
using MyStash.ResX;

namespace MyStash.Export
{
    //*******************************************************************
    //  EXPORTBASE - BASE CLASS FOR ALL EXPORT ENGINES
    //*******************************************************************	

    /// <summary>
    /// This class is the base for all export engines.
    /// It defines basic behavior and common properties.
    /// To avoid any direct use, this class is abstract.
    /// </summary>
    public abstract class ExportBase<T> : ObservableObject
    {
        #region private fields
        private Encoding charEncoding = Encoding.Unicode;
        private string defaultExtension = string.Empty;

        // When set to <c>true</c>, EndOfProcessRequested stops the export process
        private bool endOfProcessRequested;

        private string exportFileName = string.Empty;
        private FieldNameType fieldNameLegend = FieldNameType.FieldName;
        private int maxOutputRecords;
        private StreamWriter outputStream;
        private bool overWriteOutput;
        private bool suppliedStream;
        private bool removeDiacritics;

        // internal randomizer.
        private Random randomizer;

        private bool randomMode;
        private double randomPercent = 100d;
        private bool shellExecute;
        private IEnumerable<T> source;

        /// <summary>
        /// Last Export start time
        /// </summary>
        protected DateTime StartTime = DateTime.Now;

        /// <summary>
        /// Last Export end time.
        /// (this is the real end time of the whole process. So, for example, within
        /// the <see cref="WriteFooter"/> method, you must used <see cref="DateTime"/>.Now if you need to display
        /// export duration relative to <see cref="StartTime"/>).
        /// </summary>
        protected DateTime EndTime = DateTime.Now;


        /// <summary>
        /// Fields information.
        /// </summary>
        protected readonly Dictionary<string, FieldInformation> FieldsInfo = new Dictionary<string, FieldInformation>();
        /// <summary>
        /// Number of items in the source.
        /// </summary>
        protected int SourceCount;
        private FieldOrProperty fieldOrProperty = FieldOrProperty.PropertiesOrFields;
        private Destination destination;

        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportBase{T}"/>
        /// class.
        /// </summary>
        protected ExportBase()
        {
            FormatName = string.Empty;
            buildFieldsInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportBase{T}"/> class.
        /// </summary>
        /// <param name="fieldProperty">The field or Property selector.</param>
        protected ExportBase(FieldOrProperty fieldProperty)
        {
            FormatName = string.Empty;
            fieldOrProperty = fieldProperty;
            buildFieldsInfo();
        }

        #endregion

        #region public properties

       public Destination Destination
        {
            get { return destination; }
            set { Set(ref destination, value); }
        }

        /// <summary>
        /// if true, removes diacritics from string (whatever the current encoding).
        /// </summary>
        public bool RemoveDiacritics
        {
            get { return removeDiacritics; }
            set { Set(ref removeDiacritics, value); }
        }

        /// <summary>
        /// Default file extension for current export engine.
        /// </summary>
        public string DefaultExtension
        {
            get { return defaultExtension; }
            set
            {
                defaultExtension = value != null ? value.Trim() : string.Empty;
                Set(ref defaultExtension, value);
            }
        }

        /// <summary>
        /// Export output file name. If not extension is supplied, engine will use the default one.
        /// </summary>
        public string ExportFileName
        {
            get { return exportFileName; }
            set
            {
                exportFileName = value != null ? value.Trim() : string.Empty;
                Set(ref exportFileName, value);
            }
        }

        /// <summary>
        /// If <c>false</c> (default) the output file is not overwritten if it
        /// already exists. Must be set to <c>true</c> to overwrite existing output
        /// file.
        /// </summary>
        public bool OverWriteOutput
        {
            get { return overWriteOutput; }
            set { Set(ref overWriteOutput, value); }
        }

        /// <summary>
        /// Data source.
        /// </summary>
        public IEnumerable<T> Source
        {
            get { return source; }
            set
            {
                Set(ref source, value);
                SourceCount = (int)source?.Count();
            }
        }

        /// <summary>
        /// Count of exported records (can be read after a successful call to Execute).
        /// </summary>
        public int ExportedCount { get; private set; }

        /// <summary>
        /// When ShellExecute is set to <c>true</c> the output file is executed by shell at the end of process.
        /// </summary>
        public bool ShellExecute
        {
            get { return shellExecute; }
            set { Set(ref shellExecute, value); }
        }

        /// <summary>
        /// When Random mode is <c>true</c>, the output is randomized.
        /// </summary>
        public bool RandomMode
        {
            get { return randomMode; }
            set { Set(ref randomMode, value); }
        }

        /// <summary>
        /// When <see cref="RandomMode"/> is <c>true</c>, only RandomPercent % of records in the source are exported.
        /// Percentage must be between 0 and 100. 2 significant digits can be used (ie 27.69%).
        /// </summary>
        public double RandomPercent
        {
            get { return randomPercent; }
            set
            {
                if (Math.Abs(value - randomPercent) < 0.000001) return;
                if (!(value > 0.0) || !(value <= 100.0)) throw new EnaxosExportException(Consts.BadPercent);
                Set(ref randomPercent, value);
            }
        }

        /// <summary>
        /// MaxOutputRecord sets the maximum number of records to export.
        /// if set to zero, all the records are exported.
        /// Can be used simultaneously with Random mode.
        /// </summary>
        public int MaxOutputRecords
        {
            get { return maxOutputRecords; }
            set { Set(ref maxOutputRecords, value); }
        }

        /// <summary>
        /// Output file stream. It is automatically created and opened. 
        /// If one is supplied it will be used "as is", nor opened, nor closed, up to the caller to
        /// do this job. Once execute has finished its job, the property is reset to <c>null</c> in all cases.
        /// </summary>
        public StreamWriter OutputStream
        {
            get { return outputStream; }
            set
            {
                if (!Set(ref outputStream, value)) return;
                outputStream = value;
                suppliedStream = true;
            }
        }

        /// <summary>
        /// Character encoding for output file.
        /// </summary>
        public Encoding CharEncoding
        {
            get { return charEncoding; }
            set { Set(ref charEncoding, value); }
        }

        /// <summary>
        /// Property FieldNameLegend indicates the type of the legend
        /// used for field names in output file. Can be the physical field name (default)
        /// or the field caption (if any, else the physical name is returned)
        /// [not used in this version]
        /// </summary>
        public FieldNameType FieldNameLegend
        {
            get { return fieldNameLegend; }
            set { Set(ref fieldNameLegend, value); }
        }

        /// <summary>
        /// Property FieldsInformation is the property/field collection use to create output.
        /// You can access this collection, suppress, add member and change current info.
        /// For example you can change the FieldPosition of elements to change the layout.
        /// </summary>
        public Dictionary<string, FieldInformation> FieldsInformation => FieldsInfo;

        /// <summary>
        /// Indicates which part of the source items is read : fields or properties.
        /// Default behavior is : read properties, if none try to get fields.
        /// </summary>
        public FieldOrProperty FieldOrProperty
        {
            get { return fieldOrProperty; }
            set
            {
                if (Set(ref fieldOrProperty, value))
                    buildFieldsInfo(); // rebuild fields info.
            }
        }

        /// <summary>
        /// Gets or sets the display name of the format.
        /// </summary>
        /// <value>The name of the format.</value>
        public string FormatName { get; protected set; }

        #endregion

        #region public events

        /// <summary>
        /// This event is fired when the export process is started.
        /// </summary>
        public event EventHandler OnExportStart;

        /// <summary>
        /// This event is fired during process (to display a wait screen or a progress bar).
        /// It reports the number of rows to export and the current row number.
        /// When a maximum number of rows is set and/or a percentage of rows to export is set the values reported can be out of sync.
        /// </summary>
        public event EventHandler<ExportProgressEventArgs> OnProgress;

        /// <summary>
        /// This event is fired when the export process is terminated.
        /// </summary>
        public event EventHandler OnExportEnd;

        #endregion

        #region global mecanism
        /// <summary>
        /// Builds the fields info.
        /// </summary>
        private void buildFieldsInfo()
        {
            FieldsInfo.Clear();
            var pos = 10;
            var type = typeof(T);

            var arr = type.GetRuntimeProperties();
            foreach (var info in arr)
            {
                var inf = info;
                if (!inf.CanRead) continue;
                FieldsInfo.Add(inf.Name,
                               new FieldInformation
                               {
                                   IsHidden = inf.CanRead,
                                   Name = inf.Name,
                                   Type = inf.PropertyType,
                                   FieldPosition = pos,
                                   ProInfo = inf,
                                   FieldInfo = null,
                                   IsProperty = true,
                                   IsExported = true
                               });
                pos += 10;
            }
            CheckFieldTypes();
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="info">The field info.</param>
        /// <param name="row">The row.</param>
        /// <returns>value as object</returns>
        protected object GetFieldValue(FieldInformation info, T row)
        {
            try
            {
                if (info.GetFieldCallback != null)
                {
                    // caller supplied a callback to get the value
                    return info.GetFieldCallback(info, row);
                }
                return info.IsProperty
                           ? info.ProInfo.GetValue(row, null)
                           : info.FieldInfo.GetValue(row);
            }
            catch (Exception e)
            {
                throw new EnaxosExportException(string.Format(AppResources.ExportBase_GetFieldValue_Can_t_read_property__0_, info.Name), e);
            }
        }

        /// <summary>
        /// Gets the visible fields ordered by ascending field position
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<FieldInformation> GetVisibleFields()
        {
            return (from kp in FieldsInfo
                    where kp.Value.IsExported
                    orderby kp.Value.FieldPosition ascending
                    select kp.Value).ToList();
        }

        /// <summary>
        /// Checks the file name, file extension, tests overwrite permission... 
        /// If child class doesn't need any file, it must override this method.
        /// </summary>
        protected virtual void CheckFileName()
        {
            ExportFileName = ExportFileName.Trim();
            if (ExportFileName == string.Empty) throw new EnaxosExportException(Consts.EmptyFileName);
            if (Path.GetExtension(ExportFileName) == string.Empty)
                ExportFileName = Path.ChangeExtension(ExportFileName, DefaultExtension);
        }

        /// <summary>
        /// Checks if the source is valid (not <c>null</c> and not empty) else raises an exception.
        /// </summary>
        protected virtual void CheckSource()
        {
            SourceCount = 0;
            if (source == null) throw new EnaxosExportException(Consts.SourceIsNull);
            SourceCount = source.Count(); // source can have changed since Source property was set.
            if (SourceCount == 0) throw new EnaxosExportException(Consts.SourceEmpty);
        }

        /// <summary>
        /// Checks if the type of each field is compatible with current export format.
        /// array, interface and pointer are excluded.
        /// </summary>
        protected virtual void CheckFieldTypes()
        {
            foreach (var kp in FieldsInfo)
            {
                var fi = kp.Value;
                fi.IsSupportedType = !(fi.Type.IsArray || fi.Type.IsGenericParameter || fi.Type.IsPointer);
            }
        }

        /// <summary>
        /// doWriteHeader is called by Execute. It initializes the export process.
        /// </summary>
        private void doWriteHeader()
        {
            ExportedCount = 0;
            endOfProcessRequested = false;
            randomizer = randomMode ? new Random(DateTime.Now.Millisecond) : null;
            OnExportStart?.Invoke(this, null);
            WriteHeader();
        }

        /// <summary>
        /// WriteHeader can be overridden by child classes to make additional <c>init</c> before
        /// the export process really starts.
        /// </summary>
        protected virtual void WriteHeader()
        {
        }

        /// <summary>
        /// doWriteBody is called by Execute. It loops in the source to generate the output file.
        /// </summary>
        private void doWriteBody()
        {
            var ep = new ExportProgressEventArgs(Source.Count(), 0, false);

            foreach (var item in from item in source
                                 let ok = (!randomMode) || (randomMode && (randomizer.Next(0, 10000) <= randomPercent * 100.0))
                                 where ok
                                 select item)
            {
                ExportedCount++;
                if ((maxOutputRecords > 0) && (ExportedCount > maxOutputRecords)) break;
                WriteBody(item);
                var op = OnProgress;
                if (op == null) continue;
                ep.Current = ExportedCount;
                ep.EndProcessRequested = false;
                op(this, ep);
                endOfProcessRequested = ep.EndProcessRequested;
                if (endOfProcessRequested) break;
            }
        }

        /// <summary>
        /// WriteBody must be overridden by child classes to implement the export process.
        /// Parameter "row" is the current record to export.
        /// </summary>
        /// <param name="row">the item to process</param>
        protected virtual void WriteBody(T row)
        {
        }

        /// <summary>
        /// doWriteFooter is called by Execute. It ends the export process.
        /// </summary>
        private void doWriteFooter()
        {
            WriteFooter();
            var exe = OnExportEnd;
            if (exe != null) exe(this, null);
        }

        /// <summary>
        /// WriteFooter must be overridden by child classes when output format needs a footer.
        /// </summary>
        protected virtual void WriteFooter()
        {
        }


        /// <summary>
        /// CloseOutput is called by Execute, it flushes and closes the output file.
        /// </summary>
        protected virtual void CloseOutput()
        {
            try
            {
                // when stream is supplied by caller, it is not closed.
                if (outputStream == null || suppliedStream) return;
                outputStream.Flush();
                outputStream.Dispose();
            }
            finally
            {
                suppliedStream = false;
                outputStream = null;
            }
        }


        /// <summary>
        /// Executes the output file using the shell.
        /// If no application is associated with the extension, the function tries to
        /// open the file using Notepad (Windows folder is located automatically).
        /// (silverlight support is in progress)
        /// </summary>
        protected virtual void ExecuteOutput()
        {
            // not implemented
        }


        /// <summary>
        /// Returns the character set encoder depending on user choice.
        /// </summary>
        /// <returns>The encoder.</returns>
        protected virtual System.Text.Encoding GetEncoder()
        {
            System.Text.Encoding encoder;

            switch (charEncoding)
            {
                case Encoding.Unicode:
                    encoder = System.Text.Encoding.Unicode;
                    break;
                default:
                    encoder = new UTF8Encoding();
                    break;
            }
            return encoder;
        }

        /// <summary>
        /// Creates the output stream.
        /// If no output file is needed, the child class must override this method.
        /// </summary>
        protected virtual void CreateOutput()
        {
            /* if (suppliedStream && outputStream != null) return; // stream supplied by caller for this export
             var encoder = GetEncoder();
             outputStream = encoder != null
                 ? new StreamWriter(exportFileName, false, encoder)
                 : new StreamWriter(exportFileName);*/
        }

        /// <summary>
        /// Main execute method. It defines the export process and normally has not
        /// to be overridden. ("template method" design pattern g.o.f.)
        /// </summary>
        public virtual void Execute()
        {
            StartTime = DateTime.Now;
            CheckFileName(); // checks file name validity
            CheckSource(); // checks source validity
            CheckFieldTypes(); // checks fields validity for current export
            CreateOutput(); // Creates the output file stream
            try
            {
                doWriteHeader(); // writes the output file header
                doWriteBody(); // writes the output file body
                doWriteFooter(); // writes the output file footer
            }
            finally
            {
                CloseOutput(); // close the output file stream
                EndTime = DateTime.Now;
            }
            if (shellExecute) ExecuteOutput(); // if shell execute, executes the output stream
        }

        /// <summary>
        /// <see cref="VerifyQuote"/> checks input string for quotes. If any is found it is doubled
        /// using the "doubling" string.
        /// The function can used to double any character (quote or anything else).
        /// </summary>
        /// <param name="txt">Input text</param>
        /// <param name="quote">The quote char that must be checked against input text</param>
        /// <param name="doubling">The char that "doubles" each quote if any</param>
        /// <returns></returns>
        protected virtual string VerifyQuote(string txt, char quote, char doubling)
        {
            if (txt == null)
            {
                txt = string.Empty;
            }
            if ((quote == '\0') || (doubling == '\0'))
            {
                return txt.Trim();
            }

            return txt.Replace("" + quote, "" + quote + doubling);
        }

        /// <summary>
        /// Returns the number of valid columns in the source.
        /// </summary>
        /// <returns>number of valid columns</returns>
        public virtual int ValidColCount()
        {
            return FieldsInfo.Count(kp => kp.Value.IsExported);
        }


        /// <summary>
        /// Filtered the string to remove diacritics depending on <see cref="RemoveDiacritics"/> property
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>string</returns>
        protected virtual string DiacriticsFiltered(string s)
        {
            return removeDiacritics ? s.RemoveDiacritics() : s;
        }

        #endregion
       
    }
}
