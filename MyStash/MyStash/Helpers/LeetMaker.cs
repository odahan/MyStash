using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using MyStash.ResX;

namespace MyStash.Helpers
{
    [LocalizationRequired(false)]
    public static class LeetMaker
    {

        public enum LeetStrength { None, Novice, Beginner, Connoisseur, Elitist, Expert, Master, Alien }

        public static Dictionary<LeetStrength, string> LeetStrengthStr { get; } = new Dictionary<LeetStrength, string>
                                                                                  {
                                                                                      {LeetStrength.None, AppResources.LeetMaker_LeetStrengthStr_None},
                                                                                      {LeetStrength.Novice, AppResources.LeetMaker_LeetStrengthStr_Novice},
                                                                                      {LeetStrength.Beginner, AppResources.LeetMaker_LeetStrengthStr_Beginner},
                                                                                      {LeetStrength.Connoisseur, AppResources.LeetMaker_LeetStrengthStr_Connoisseur},
                                                                                      {LeetStrength.Elitist, AppResources.LeetMaker_LeetStrengthStr_Elitist},
                                                                                      {LeetStrength.Expert, AppResources.LeetMaker_LeetStrengthStr_Expert},
                                                                                      {LeetStrength.Master, AppResources.LeetMaker_LeetStrengthStr_Master},
                                                                                      {LeetStrength.Alien, AppResources.LeetMaker_LeetStrengthStr_Alien}
                                                                                  };
    

        /// <summary>
        /// Translate text to Leet - Extension methods for string class
        /// </summary>
        /// <param name="text">Orginal text</param>
        /// <param name="strength">Degree of translation (0 - 100%)</param>
        /// <returns>Leet translated text</returns>
        public static string ToLeet(this string text, LeetStrength strength = LeetStrength.Connoisseur)
        {
            return Translate(text, strength);
        }
        /// <summary>
        /// Translate text to Leet
        /// </summary>
        /// <param name="text">Orginal text</param>
        /// <param name="strength">Degree of translation</param>
        /// <returns>Leet translated text</returns>
        public static string Translate(string text, LeetStrength strength)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            // No Leet Translator
            if (strength == LeetStrength.None) return text;
            // StringBuilder to store result.
            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                #region Novice
                if (strength==LeetStrength.Novice)
                {
                    switch (c)
                    {
                        case 'e': sb.Append("3"); break;
                        case 'E': sb.Append("3"); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region Minimal
                else if (strength==LeetStrength.Beginner)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region strong
                else if (strength==LeetStrength.Connoisseur)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        case 's': sb.Append("$"); break;
                        case 'S': sb.Append("$"); break;
                        case 'l': sb.Append("£"); break;
                        case 'L': sb.Append("£"); break;
                        case 'c': sb.Append("("); break;
                        case 'C': sb.Append("("); break;
                        case 'y': sb.Append("¥"); break;
                        case 'Y': sb.Append("¥"); break;
                        case 'u': sb.Append("µ"); break;
                        case 'U': sb.Append("µ"); break;
                        case 'd': sb.Append("Ð"); break;
                        case 'D': sb.Append("Ð"); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region very strong
                else if (strength==LeetStrength.Elitist)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        case 'k': sb.Append("|{"); break;
                        case 'K': sb.Append("|{"); break;
                        case 's': sb.Append("$"); break;
                        case 'S': sb.Append("$"); break;
                        case 'g': sb.Append("9"); break;
                        case 'G': sb.Append("9"); break;
                        case 'l': sb.Append("£"); break;
                        case 'L': sb.Append("£"); break;
                        case 'c': sb.Append("("); break;
                        case 'C': sb.Append("("); break;
                        case 't': sb.Append("7"); break;
                        case 'T': sb.Append("7"); break;
                        case 'z': sb.Append("2"); break;
                        case 'Z': sb.Append("2"); break;
                        case 'y': sb.Append("¥"); break;
                        case 'Y': sb.Append("¥"); break;
                        case 'u': sb.Append("µ"); break;
                        case 'U': sb.Append("µ"); break;
                        case 'f': sb.Append("ƒ"); break;
                        case 'F': sb.Append("ƒ"); break;
                        case 'd': sb.Append("Ð"); break;
                        case 'D': sb.Append("Ð"); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region ultra strong
                else if (strength==LeetStrength.Expert)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        case 'k': sb.Append("|{"); break;
                        case 'K': sb.Append("|{"); break;
                        case 's': sb.Append("$"); break;
                        case 'S': sb.Append("$"); break;
                        case 'g': sb.Append("9"); break;
                        case 'G': sb.Append("9"); break;
                        case 'l': sb.Append("£"); break;
                        case 'L': sb.Append("£"); break;
                        case 'c': sb.Append("("); break;
                        case 'C': sb.Append("("); break;
                        case 't': sb.Append("7"); break;
                        case 'T': sb.Append("7"); break;
                        case 'z': sb.Append("2"); break;
                        case 'Z': sb.Append("2"); break;
                        case 'y': sb.Append("¥"); break;
                        case 'Y': sb.Append("¥"); break;
                        case 'u': sb.Append("µ"); break;
                        case 'U': sb.Append("µ"); break;
                        case 'f': sb.Append("ƒ"); break;
                        case 'F': sb.Append("ƒ"); break;
                        case 'd': sb.Append("Ð"); break;
                        case 'D': sb.Append("Ð"); break;
                        case 'n': sb.Append("|\\|"); break;
                        case 'N': sb.Append("|\\|"); break;
                        case 'w': sb.Append("\\/\\/"); break;
                        case 'W': sb.Append("\\/\\/"); break;
                        case 'h': sb.Append("|-|"); break;
                        case 'H': sb.Append("|-|"); break;
                        case 'v': sb.Append("\\/"); break;
                        case 'V': sb.Append("\\/"); break;
                        case 'm': sb.Append("|\\/|"); break;
                        case 'M': sb.Append("|\\/|"); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region expert
                else if (strength==LeetStrength.Master)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        case 's': sb.Append("$"); break;
                        case 'S': sb.Append("$"); break;
                        case 'g': sb.Append("9"); break;
                        case 'G': sb.Append("9"); break;
                        case 'l': sb.Append("£"); break;
                        case 'L': sb.Append("£"); break;
                        case 'c': sb.Append("("); break;
                        case 'C': sb.Append("("); break;
                        case 't': sb.Append("7"); break;
                        case 'T': sb.Append("7"); break;
                        case 'z': sb.Append("2"); break;
                        case 'Z': sb.Append("2"); break;
                        case 'y': sb.Append("¥"); break;
                        case 'Y': sb.Append("¥"); break;
                        case 'u': sb.Append("µ"); break;
                        case 'U': sb.Append("µ"); break;
                        case 'f': sb.Append("ƒ"); break;
                        case 'F': sb.Append("ƒ"); break;
                        case 'd': sb.Append("Ð"); break;
                        case 'D': sb.Append("Ð"); break;
                        case 'n': sb.Append("|\\|"); break;
                        case 'N': sb.Append("|\\|"); break;
                        case 'w': sb.Append("\\/\\/"); break;
                        case 'W': sb.Append("\\/\\/"); break;
                        case 'h': sb.Append("|-|"); break;
                        case 'H': sb.Append("|-|"); break;
                        case 'v': sb.Append("\\/"); break;
                        case 'V': sb.Append("\\/"); break;
                        case 'k': sb.Append("|{"); break;
                        case 'K': sb.Append("|{"); break;
                        case 'r': sb.Append("®"); break;
                        case 'R': sb.Append("®"); break;
                        case 'm': sb.Append("|\\/|"); break;
                        case 'M': sb.Append("|\\/|"); break;
                        case 'b': sb.Append("ß"); break;
                        case 'B': sb.Append("ß"); break;
                        case 'q': sb.Append("Q"); break;
                        case 'Q': sb.Append("Q¸"); break;
                        case 'x': sb.Append(")("); break;
                        case 'X': sb.Append(")("); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
                #region Degree 100
                else if (strength==LeetStrength.Alien)
                {
                    switch (c)
                    {
                        case 'a': sb.Append("4"); break;
                        case 'e': sb.Append("3"); break;
                        case 'i': sb.Append("1"); break;
                        case 'o': sb.Append("0"); break;
                        case 'A': sb.Append("4"); break;
                        case 'E': sb.Append("3"); break;
                        case 'I': sb.Append("1"); break;
                        case 'O': sb.Append("0"); break;
                        case 's': sb.Append("$"); break;
                        case 'S': sb.Append("$"); break;
                        case 'g': sb.Append("9"); break;
                        case 'G': sb.Append("9"); break;
                        case 'l': sb.Append("£"); break;
                        case 'L': sb.Append("£"); break;
                        case 'c': sb.Append("("); break;
                        case 'C': sb.Append("("); break;
                        case 't': sb.Append("7"); break;
                        case 'T': sb.Append("7"); break;
                        case 'z': sb.Append("2"); break;
                        case 'Z': sb.Append("2"); break;
                        case 'y': sb.Append("¥"); break;
                        case 'Y': sb.Append("¥"); break;
                        case 'u': sb.Append("µ"); break;
                        case 'U': sb.Append("µ"); break;
                        case 'f': sb.Append("ƒ"); break;
                        case 'F': sb.Append("ƒ"); break;
                        case 'd': sb.Append("Ð"); break;
                        case 'D': sb.Append("Ð"); break;
                        case 'n': sb.Append("|\\|"); break;
                        case 'N': sb.Append("|\\|"); break;
                        case 'w': sb.Append("\\/\\/"); break;
                        case 'W': sb.Append("\\/\\/"); break;
                        case 'h': sb.Append("|-|"); break;
                        case 'H': sb.Append("|-|"); break;
                        case 'v': sb.Append("\\/"); break;
                        case 'V': sb.Append("\\/"); break;
                        case 'k': sb.Append("|{"); break;
                        case 'K': sb.Append("|{"); break;
                        case 'r': sb.Append("®"); break;
                        case 'R': sb.Append("®"); break;
                        case 'm': sb.Append("|\\/|"); break;
                        case 'M': sb.Append("|\\/|"); break;
                        case 'b': sb.Append("ß"); break;
                        case 'B': sb.Append("ß"); break;
                        case 'j': sb.Append("_|"); break;
                        case 'J': sb.Append("_|"); break;
                        case 'P': sb.Append("|°"); break;
                        case 'q': sb.Append("¶"); break;
                        case 'Q': sb.Append("¶¸"); break;
                        case 'x': sb.Append(")("); break;
                        case 'X': sb.Append(")("); break;
                        default: sb.Append(c); break;
                    }
                }
                #endregion
            }
            return sb.ToString(); // Return result.
        }
    }
}
