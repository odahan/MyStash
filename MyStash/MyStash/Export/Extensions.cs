using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MyStash.Export
{
    /// <summary>
    /// String extensions
    /// </summary>
    [LocalizationRequired(false)]
    public static class Extensions
    {
        /// <summary>
        /// Filters ASCII characters.
        /// </summary>
        /// <param name="name">The string to filter.</param>
        /// <returns>filtered string</returns>
        public static string OnlyCharAndDigit(this string name)
        {
            var tmp = new StringBuilder(name.Length);
            foreach (var t in name.ToCharArray().Where(char.IsLetterOrDigit))
            {
                tmp.Append(t);
            }
            return tmp.ToString();
        }

        /// <summary>
        /// Determines whether the specified STR number is a natural number.
        /// </summary>
        /// <param name="strNumber">The string.</param>
        /// <returns>
        /// 	<c>true</c> if it is a natural number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNaturalNumber(this string strNumber)
        {
            var objNotNaturalPattern = new Regex("[^0-9]");
            var objNaturalPattern = new Regex("0*[1-9][0-9]*");

            return !objNotNaturalPattern.IsMatch(strNumber) &&
                   objNaturalPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test for Positive Integers with zero inclusive
        /// </summary>
        /// <param name="strNumber">string to test</param>
        /// <returns><c>true</c> if string is a positive integers with zero inclusive</returns>
        public static bool IsWholeNumber(this string strNumber)
        {
            var objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test for Integers both Positive and Negative
        /// </summary>
        /// <param name="strNumber">string to test</param>
        /// <returns><c>true</c> if string is an integer (positive or negative)</returns>
        public static bool IsInteger(this string strNumber)
        {
            var objNotIntPattern = new Regex("[^0-9-]");
            var objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !objNotIntPattern.IsMatch(strNumber) &&
                   objIntPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to Test for Positive Number both Integer and Real
        /// </summary>
        /// <param name="strNumber">string to test</param>
        /// <returns><c>true</c> if string is a positive integer or real</returns>
        public static bool IsPositiveNumber(this string strNumber)
        {
            var objNotPositivePattern = new Regex("[^0-9.]");
            var objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            var objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");

            return !objNotPositivePattern.IsMatch(strNumber) &&
                   objPositivePattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test whether the string is valid number or not
        /// </summary>
        /// <param name="strNumber">string to test</param>
        /// <returns><c>true</c> if string is a valid number</returns>
        public static bool IsNumber(this string strNumber)
        {
            var objNotNumberPattern = new Regex("[^0-9.-]");
            var objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            var objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            const string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            const string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            var objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Function to test for Alphabets.
        /// </summary>
        /// <param name="strToCheck">string to test</param>
        /// <returns><c>true</c> if string is only alpha</returns>
        public static bool IsAlpha(this string strToCheck)
        {
            var objAlphaPattern = new Regex("[^a-zA-Z]");
            return !objAlphaPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Function to Check for AlphaNumeric.
        /// </summary>
        /// <param name="strToCheck">string to check</param>
        /// <returns><c>true</c> if string is alphanumeric only</returns>
        public static bool IsAlphaNumeric(this string strToCheck)
        {
            var objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");

            return !objAlphaNumericPattern.IsMatch(strToCheck);
        }

        /// <summary>
        /// Pads a text or truncates it when it's too long.
        /// </summary>
        /// <param name="txt">the text to pad</param>
        /// <param name="len">the length of the padded text</param>
        /// <param name="rightNum">if <c>true</c>, right padding</param>
        /// <returns></returns>
        public static string Pad(this string txt, int len, bool rightNum)
        {
            txt = txt.Trim();
            if (txt.Length > len) return txt.Substring(0, len);
            if (rightNum && (len > txt.Length) && txt.IsNumber())
                return txt.PadLeft(len, ' ');
            return txt.PadRight(len, ' ');
        }

        /// <summary>
        /// Suppresses characters under ascii 32 and transforms CR to space
        /// </summary>
        /// <param name="txt">text to transform</param>
        /// <returns>filtered text</returns>
        public static string NoControl(this string txt)
        {
            txt = txt.Replace('\r', ' '); 
            var s = new StringBuilder(txt.Length);
            foreach (var c in txt)
                if (c >= 32) s.Append(c);
            return s.ToString();
        }

        /// <summary>
        /// Change the currency decimal separator to "."
        /// </summary>
        /// <param name="txt">the string containing the float</param>
        /// <returns>modified input string</returns>
        public static string ChangeDecimal(this string txt)
        {
            var i = txt.IndexOf(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator, StringComparison.Ordinal);
            if (i <= -1) return txt;
            var t = txt.Remove(i, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator.Length);
            t = t.Insert(i, ".");
            return t;
        }

        /// <summary>
        /// Encodes a string to change any non ascii char to its corresponding
        /// SGML code.
        /// </summary>
        /// <param name="text">The text to encode</param>
        /// <param name="full">When pure=<c>true</c>, if full=<c>true</c> "less
        /// than" and "more than" symbols are also filtered. If <c>false</c>
        /// these chars are not encoded.</param>
        /// <param name="pure">If <c>true</c> special chars (as <c>ç é</c>...)
        /// are translated into their SGML code, if <c>false</c> any non ascii char is
        /// encoded using its ascii code.</param>
        /// <returns>The encoded version of input parameter "text"</returns>
        public static string Text2sgml(this string text, bool full, bool pure)
        {
            //return text;
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (!pure)
            {
                for (var i = text.Length - 1; i >= 0; i--)
                {
                    int c = Convert.ToInt16(text[i]);
                    if ((c < 32) || (text[i] == '"') || (text[i] == '&')
                        || (text[i] == '<') || (text[i] == '>')
                        || (c > 126))
                    {
                        text = text.Remove(i, 1);
                        text = text.Insert(i, "&#" + c + ";");
                    }
                }
                return text;
            }
            var b = new StringBuilder(text, text.Length*2);

            // this code is very heavy. But how to improve it ?
            //agentsmith spellcheck disable
            b.Replace("&", "##amp;");
            b.Replace("" + (char)255, "&yuml;");
            b.Replace("þ", "&thorn;");
            b.Replace("×", "&times;");
            b.Replace("" + (char)160, "&nbsp;");
            b.Replace("¡", "&iexcl;");
            b.Replace("¢", "&cent;");
            b.Replace("£", "&pound;");
            b.Replace("¤", "&curren;");
            b.Replace("¥", "&yen;");
            b.Replace("¦", "&brvbar;");
            b.Replace("§", "&sect;");
            b.Replace("¨", "&uml;");
            b.Replace("©", "&copy;");
            b.Replace("ª", "&ordf;");
            b.Replace("«", "&laquo;");
            b.Replace("¬", "&not;");
            b.Replace("­", "&shy;");
            b.Replace("®", "&reg;");
            b.Replace("¯", "&macr;");
            b.Replace("°", "&deg;");
            b.Replace("±", "&plusmn;");
            b.Replace("²", "&sup2;");
            b.Replace("³", "&sup3;");
            b.Replace("´", "&acute;");
            b.Replace("µ", "&micro;");
            b.Replace("¶", "&para;");
            b.Replace("·", "&middot;");
            b.Replace("¸", "&cedille;");
            b.Replace("¹", "&sup1;");
            b.Replace("º", "&ordm;");
            b.Replace("»", "&raquo;");
            b.Replace("¼", "&frac14;");
            b.Replace("½", "&frac12;");
            b.Replace("¾", "&frac34;");
            b.Replace("¿", "&iquest;");
            b.Replace("å", "&aring;");
            b.Replace("Å", "&Aring;");
            b.Replace("ä", "&auml;");
            b.Replace("Ä", "&Auml;");
            b.Replace("á", "&aacute;");
            b.Replace("Á", "&Aacute;");
            b.Replace("à", "&agrave;");
            b.Replace("À", "&Agrave;");
            b.Replace("æ", "&aelig;");
            b.Replace("Æ", "&Aelig;");
            b.Replace("Â", "&Acirc;");
            b.Replace("â", "&acirc;");
            b.Replace("ã", "&atilde;");
            b.Replace("Ã", "&Atilde;");
            b.Replace("ç", "&ccedil;");
            b.Replace("Ç", "&Ccedil;");
            b.Replace("é", "&eacute;");
            b.Replace("É", "&Eacute;");
            b.Replace("ê", "&ecirc;");
            b.Replace("Ê", "&Ecirc;");
            b.Replace("ë", "&euml;");
            b.Replace("Ë", "&Euml;");
            b.Replace("è", "&egrave;");
            b.Replace("È", "&Egrave;");
            b.Replace("î", "&icirc;");
            b.Replace("Î", "&Icirc;");
            b.Replace("í", "&iacute;");
            b.Replace("Í", "&Iacute;");
            b.Replace("ì", "&igrave;");
            b.Replace("Ì", "&Igrave;");
            b.Replace("ï", "&iuml;");
            b.Replace("Ï", "&Iuml;");
            b.Replace("ñ", "&ntilde;");
            b.Replace("Ñ", "&Ntilde;");
            b.Replace("ö", "&ouml;");
            b.Replace("Ö", "&Ouml;");
            b.Replace("ò", "&ograve;");
            b.Replace("Ò", "&Ograve;");
            b.Replace("ó", "&oacute;");
            b.Replace("Ó", "&Oacute;");
            b.Replace("ø", "&oslash;");
            b.Replace("Ø", "&Oslash;");
            b.Replace("Ô", "&Ocirc;");
            b.Replace("ô", "&ocirc;");
            b.Replace("õ", "&otilde;");
            b.Replace("Õ", "&Otilde;");
            b.Replace("ü", "&uuml;");
            b.Replace("Ü", "&Uuml;");
            b.Replace("ú", "&uacute;");
            b.Replace("Ú", "&Uacute;");
            b.Replace("Ù", "&Ugrave;");
            b.Replace("ù", "&ugrave;");
            b.Replace("û", "&ucirc;");
            b.Replace("Û", "&Ucirc;");
            b.Replace("ý", "&yacute;");
            b.Replace("Ý", "&Yacute;");
            b.Replace("ÿ", "&yuml;");
            b.Replace("|", "&nbsp;");
            b.Replace("\"", "&quot;");
            b.Replace("" + ((char)13) + ((char)10), "" + (char)10);
            b.Replace("" + (char)13, "");

            if (full)
            {
                b.Replace("<", "&lt;");
                b.Replace(">", "&gt;");
            }

            b.Replace("##amp", "&amp");
            b.Replace("" + (char)10, "<br>");

            for (var i = b.Length - 1; i >= 0; i--)
            {
                int c = Convert.ToInt16(b[i]);
                if (c >= 32) continue;
                b.Remove(i, 1);
                b.Insert(i, "&#" + c + ";");
            }

            return b.ToString();
            //agentsmith spellcheck restore
        }

        /// <summary>
        /// Removes the diacritics.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>normalized non diacritic string</returns>
        public static string RemoveDiacritics(this string s)
        {
            //var normalizedString = s. Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in
                s.ToCharArray().Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            {
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        } 

    }
}
