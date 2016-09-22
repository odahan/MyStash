using System;
using System.Globalization;

namespace MyStash.Export
{
    //*******************************************************************
    //  EXPORTSQL - SQL EXPORT CLASS
    //*******************************************************************

    /// <summary>
    /// Export to SQL script file.
    /// Generates series of "INSERT INTO"
    /// </summary>
    //public class ExportSql<T,TT> : ExportBase<T,TT> where T : IEnumerable<TT> 
    public class ExportSql<T> : ExportBase<T>
    {
        #region private fields
        private bool addStringPrefix;
        private string commentEnd;
        private string commentMarker;
        private string dateTimeFmt;
        private string endOfLineMarker;
        private string fieldList;
        private string fieldNameClosingQuote;
        private string fieldNameOpeningQuote;
        private string footer;
        private bool footerAsComment;
        private string header;
        private bool headerAsComment;
        private string nullKeyword;
        private string stringPrefix;
        private string stringQuote;
        private string stringQuoteClosing;
        private string tableName;
        private const string Defaultstringquote = "\"";
        private const string Defaulttablename = "InfoSheet"; 
        #endregion

        #region properties
        /// <summary>
        /// Header to be added to output SQL script.
        /// </summary>
        public string Header
        {
            get { return header; }
            set
            {
                if (header==value) return;
                header = value != null ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The prefix is added to string data before the quote.
        /// With some RDBMS it is necessary to add "N" for Unicode character set (ex: <c>Sql</c> Server)
        /// </summary>
        public string StringPrefix
        {
            get { return stringPrefix; }
            set
            {
                if (value==stringPrefix) return;
                stringPrefix = (value != null) ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If <c>true</c> the <see cref="StringPrefix"/> is added to string values.
        /// </summary>
        public bool AddStringPrefix
        {
            get { return addStringPrefix; }
            set
            {
                if (value==addStringPrefix) return;
                addStringPrefix = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// if <c>true</c> header is treated as a comment. Else the header is output
        /// exactly as typed.
        /// </summary>
        public bool HeaderAsComment
        {
            get { return headerAsComment; }
            set
            {
                if (value==headerAsComment) return;
                headerAsComment = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Footer to be added at end of SQL script file.
        /// </summary>
        public string Footer
        {
            get { return footer; }
            set
            {
                if (value==footer) return;
                footer = value != null ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If <c>true</c> the footer is treated as a comment. Else the footer is
        /// output exactly as typed.
        /// </summary>
        public bool FooterAsComment
        {
            get { return footerAsComment; }
            set
            {
                if (value == footerAsComment) return;
                footerAsComment = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The name of the destination table. Used to generate "Insert into" SQL statements.
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set
            {
                if (value==tableName) return;
                tableName = value != null ? value.Trim() : Defaulttablename;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Opening quote for string data. if closing quote is not defined the StringQuote is use at both string ends.
        /// </summary>
        public string StringQuote
        {
            get { return stringQuote; }
            set
            {
                if (value==StringQuote) return;
                stringQuote = value != null ? value.Trim() : Defaultstringquote;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Closing string quote. Optional (default none).
        /// For example, for SQL Server, the OpeningQuote is "[" and the ClosingQuote is "]".
        /// For <c>Interbase</c> or <c>Firebird</c>, only OpeningQuote is needed (a double quote).
        /// </summary>
        public string StringQuoteClosing
        {
            get { return stringQuoteClosing; }
            set
            {
                if (value==stringQuote) return;
                stringQuoteClosing = value ?? String.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Symbols to open a comment line (default "--")
        /// </summary>
        public string CommentMarker
        {
            get { return commentMarker; }
            set
            {
                if (value==commentMarker) return;
                commentMarker = value != null ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Symbols to close a comment line (default = string.empty)
        /// </summary>
        public string CommentEnd
        {
            get { return commentEnd; }
            set
            {
                if (value==commentEnd) return;
                commentEnd = value != null ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Character or string used as opening quote for field names. default none
        /// If no closing quote are defined, the opening quote is used for both opening and closing.
        /// </summary>
        public string FieldNameOpeningQuote
        {
            get { return fieldNameOpeningQuote; }
            set
            {
                if (value == fieldNameOpeningQuote) return;
                fieldNameOpeningQuote = value ?? String.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Character or string use as closing quote for field names. default none
        /// </summary>
        public string FieldNameClosingQuote
        {
            get { return fieldNameClosingQuote; }
            set
            {
                if (value==fieldNameClosingQuote) return;
                fieldNameClosingQuote = value ?? String.Empty;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// Format string for <see cref="DateTime"/> values.
        /// </summary>
        public string DateTimeFormat
        {
            get { return dateTimeFmt; }
            set
            {
                if (value==dateTimeFmt) return;
                dateTimeFmt = (value != null) ? value.Trim() : string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// End of line marker. The string is added at end of each line in the script.
        /// </summary>
        public string EndOfLineMarker
        {
            get { return endOfLineMarker; }
            set
            {
                if (value==endOfLineMarker) return;
                endOfLineMarker = value ?? String.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// <c>null</c> keyword used to insert a <c>null</c> data.
        /// </summary>
        public string NullKeyword
        {
            get { return nullKeyword; }
            set
            {
                if (value==nullKeyword) return;
                nullKeyword = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportSql&lt;T&gt;"/> class.
        /// </summary>
        public ExportSql()
        {
            header = string.Empty;
            footer = string.Empty;
            tableName = Defaulttablename;
            stringQuote = "'";// Defaultstringquote;
            stringQuoteClosing = "'";
            DefaultExtension = ".sql";
            commentMarker = "--";
            nullKeyword = "null";
            dateTimeFmt = "yyyy/MM/dd hh:mm:sss";
            stringPrefix = string.Empty; // "N" with SQL server
            commentEnd = string.Empty;
            fieldNameClosingQuote = "]";
            fieldNameOpeningQuote = "[";
            endOfLineMarker = ";";
            headerAsComment = false;
            footerAsComment = false;
            addStringPrefix = false;
            FormatName = "SQL";
        } 
        #endregion

        #region overriden methods
        /// <summary>
        /// Creates file header and stores the list of fields.
        /// </summary>
        protected override void WriteHeader()
        {
            OutputStream.WriteLine(commentMarker + " " + DiacriticsFiltered(Consts.GeneratedBy) + " - " + StartTime + " " +
                                   commentEnd);
            if (header != string.Empty)
            {
                if (headerAsComment)
                    OutputStream.WriteLine(commentMarker + " " + DiacriticsFiltered(header) + " " + commentEnd);
                else
                    OutputStream.WriteLine(header);
            }
            
            fieldList = string.Empty;
            foreach (var field in GetVisibleFields())
            {
                if (fieldList.Length > 0) fieldList += ", ";
                fieldList += fieldNameOpeningQuote + DiacriticsFiltered(field.ExportName).OnlyCharAndDigit() + fieldNameClosingQuote;
            }
            if (tableName == String.Empty) tableName = Defaulttablename;
            OutputStream.Flush();
        }

        /// <summary>
        /// Generates a line of script.
        /// </summary>
        /// <param name="row"></param>
        protected override void WriteBody(T row)
        {
            OutputStream.Write("INSERT INTO " + DiacriticsFiltered(tableName) + "\r\n" +
                               "(" + fieldList + ")\r\n" +
                               "VALUES\r\n" +
                               "(");
            var line = string.Empty;
            var theQuote = (stringQuote != string.Empty) ? stringQuote[0] : '\0';
            foreach (var field in GetVisibleFields())
            {
                string s;
                var objvalue = GetFieldValue(field, row);
                if (field.IsReal)
                {
                    var inf = new CultureInfo("en-US").NumberFormat;
                    inf.NumberDecimalSeparator = ".";
                    inf.NumberGroupSeparator = "";
                    s = (objvalue != null) ? Convert.ToDecimal(objvalue).ToString("N6", inf) : NullKeyword;
                }
                else if (field.IsInteger)
                    s = (objvalue != null) ? objvalue.ToString() : NullKeyword;
                else if (field.IsDateTime)
                    s = Convert.ToDateTime(objvalue).ToString(dateTimeFmt);
                else
                {
                    if (objvalue == null) s = NullKeyword;
                    else
                    {
                        s = VerifyQuote(DiacriticsFiltered(objvalue.ToString()).NoControl(), theQuote, theQuote);
                        if (stringQuoteClosing == string.Empty) s = stringQuote + s + stringQuote;
                        else s = stringQuote + s + stringQuoteClosing;
                        if (addStringPrefix) s = StringPrefix + s;
                    }
                }

                if (line != string.Empty)
                {
                    line += ", ";
                }
                line += s;
            }
            OutputStream.WriteLine(line + ")" + endOfLineMarker);
            OutputStream.Flush();
        }

        /// <summary>
        /// Ends the script (adds the footer if needed).
        /// </summary>
        protected override void WriteFooter()
        {
            OutputStream.WriteLine(commentMarker + " " + 
                DiacriticsFiltered(string.Format("there are {0} records in this file.",ExportedCount,SourceCount,StartTime-EndTime))+
                " " +commentEnd);
            if (footer != string.Empty)
            {
                if (footerAsComment)
                    OutputStream.WriteLine(commentMarker + " " + DiacriticsFiltered(footer) + " " + commentEnd);
                else
                    OutputStream.WriteLine(DiacriticsFiltered(footer));
            }
            OutputStream.Flush();
        }

        #endregion
    }
}
