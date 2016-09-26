using System;
using System.ComponentModel;

namespace MyStash.Export
{
    //*******************************************************************
    //  EXPORTCSV - CSV EXPORT CLASS
    //*******************************************************************

    /// <summary>
    /// Export to CSV format.
    /// </summary>
    public class ExportCsv<T> : ExportBase<T>
    {
        private string dateTimeFmt;
        private string intFmt;
        private bool padData;
        private string quote;
        private string realFmt;
        private string separator;
        private bool titleRow;


        /// <summary>
        /// Format string for <see cref="DateTime"/> (date, time, <c>datetime</c>).
        /// </summary>
        //agentsmith spellcheck disable
        //agentsmith spellcheck restore
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
        /// Used to delimit values.
        /// </summary>
        public string Quote
        {
            get { return quote; }
            set
            {
                if (value==quote) return;
                quote = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Used to separate values. 
        /// Known escape sequences : T tab, N new line, R carriage return, F form feed, U<c>xxx</c> Unicode value.
        /// </summary>
        public string Separator
        {
            get
            {
                if ((!separator.StartsWith(@"\")) || (separator.Length <= 1)) return separator;
                var c = separator.ToUpper()[1];
                switch (c)
                {
                    case 'T':
                        return ((char)9) + "";
                    case 'N':
                        return ((char)10) + "";
                    case 'R':
                        return ((char)13) + "";
                    case 'F':
                        return ((char)12) + "";
                    case 'U':
                        return ((char)Convert.ToInt16(separator.Substring(1, separator.Length))) + "";
                    default:
                        return separator;
                }
            }
            set
            {
                if (value==separator) return;
                separator = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If set to <c>true</c>, a header with field names is added.
        /// </summary>
        public bool TitleRow
        {
            get { return titleRow; }
            set
            {
                if (value==titleRow) return;
                titleRow = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If set to <c>true</c> data are padded with spaces.
        /// </summary>
        public bool PadData
        {
            get { return padData; }
            set
            {
                if (value==padData) return;
                padData = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Format string for integer data (integer, byte, unsigned integer...).
        /// </summary>
        public string IntegerFormat
        {
            get { return intFmt; }
            set
            {
                if (value==intFmt) return;
                intFmt = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Format string for real data (double, single, decimal).
        /// </summary>
        public string RealFormat
        {
            get { return realFmt; }
            set
            {
                if (value==RealFormat) return;
                realFmt = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCsv{T}"/> class.
        /// </summary>
        public ExportCsv()
        {
            padData = false;
            titleRow = true;
            //agentsmith spellcheck disable
            DefaultExtension = ".csv";
            quote = "\"";
            separator = ";";
            realFmt = "";
            intFmt = "";
            dateTimeFmt = "yyyy/MM/dd hh:mm:sss";
            //agentsmith spellcheck restore
            CharEncoding = Encoding.Ansi;
            FormatName = "CSV";
        }

        /// <summary>
        /// Creates file header.
        /// </summary>
        protected override void WriteHeader()
        {
            if (!titleRow) return;
            var s = string.Empty;
            foreach (var field in GetVisibleFields())
            {
                if (s.Length > 0) s += Separator;
                if (padData) s += DiacriticsFiltered(field.ExportName).OnlyCharAndDigit().Pad(field.DisplayWidth, field.IsNumber);
                else s += quote + DiacriticsFiltered(field.ExportName).OnlyCharAndDigit() + quote;
            }
            OutputStream.WriteLine(s);
            OutputStream.Flush();
        }

        /// <summary>
        /// Pads data if needed (depending on data type).
        /// </summary>
        /// <param name="data">the string to pad</param>
        /// <param name="field">The corresponding <see cref="FieldInformation"/> object</param>
        /// <returns>Padded data if needed, else filtered data</returns>
        private string padout(string data, FieldInformation field)
        {
            var rslt = !padData ? data : data.Pad(field.DisplayWidth, field.IsNumber).NoControl();
            //return (VerifyQuote(rslt, '"', '"').NoControl());
            return rslt.Replace(quote,"''"); // adptation for mystash
        }

        /// <summary>
        /// Generates a record row in output file.
        /// </summary>
        /// <param name="row">The row to export</param>
        protected override void WriteBody(T row)
        {
            var i = 0;
            foreach (var field in GetVisibleFields())
            {
                var objvalue = GetFieldValue(field, row);
                if (i > 0) OutputStream.Write(Separator);
                if (objvalue == null) continue;
                if (field.IsInteger) OutputStream.Write((Convert.ToInt64(objvalue).ToString(intFmt)));
                else if (field.IsReal) OutputStream.Write((Convert.ToDouble(objvalue).ToString(realFmt)));
                else if (field.IsDateTime)
                    OutputStream.Write((Convert.ToDateTime(objvalue).ToString(dateTimeFmt)));
                else OutputStream.Write(quote + padout(DiacriticsFiltered(objvalue.ToString()), field) + quote);
                i++;
            }
            OutputStream.WriteLine();
            OutputStream.Flush();
        }
    }
}
