using System;
using System.Collections.Generic;
using System.Linq;
using MyStash.ResX;

namespace MyStash.Helpers
{
    /// <summary>
    /// Determines how strong a password is based on lots of different criteria. 0 is very weak. 100 is Very strong.
    /// The password to check is passed as a property, and other properties can then be used to get the score, the
    /// password strength (as a comment). 
    /// Some other properties allow to get score detail (bonus, malus) and one other can return a full analysis
    /// of the password explaining why it is scoring as it does.
    /// From http://www.codeproject.com/KB/miscctrl/PasswordStrengthControl.aspx heavily modified 7/2010.
    /// This version : 6/2016 crossplatform Oliver Dahan http://www.e-naxos.com/blog 
    /// </summary>
    public class PasswordChecker
    {
        private PasswordChecker()
        { }

        public static PasswordChecker Instance { get; } = new PasswordChecker();

        private List<AnalysisItem> dtDetails;
        private string password = string.Empty;


        /// <summary>
        /// Gets or sets the password to check.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value) return;
                password = value;
                CheckPasswordWithDetails(password);
            }
        }

        /// <summary>
        /// Gets the password score.
        /// 0 = weak, 100 = very strong
        /// </summary>
        /// <value>The password score.</value>
        public int PasswordScore => dtDetails?[0].Total ?? 0;

        /// <summary>
        /// Gets the password bonus.
        /// </summary>
        /// <value>The password bonus.</value>
        public int PasswordBonus => dtDetails?[0].Bonus ?? 0;

        /// <summary>
        /// Gets the password malus.
        /// </summary>
        /// <value>The password malus.</value>
        public int PasswordMalus => dtDetails?[0].Malus ?? 0;


        /// <summary>
        /// Returns a textual description of the strength of the password
        /// </summary>
        /// <returns></returns>
        public string PasswordStrength => dtDetails != null ? dtDetails[0].Rate : AppResources.PasswordChecker_PasswordStrength_Unknown;

        /// <summary>
        /// Returns the details for the password score - Allows you to see why a password has the score it does.
        /// </summary>
        /// <returns></returns>
        public List<AnalysisItem> StrengthDetails => dtDetails;

        /// <summary>
        /// This is the method which checks the password and determines the score.
        /// </summary>
        /// <param name="pwd"></param>
        public void CheckPasswordWithDetails(string pwd)
        {
            // Init Vars
            var nMalus = 0;
            var sComplexity = "";
            var iUpperCase = 0;
            var iLowerCase = 0;
            var iDigit = 0;
            var iSymbol = 0;
            var iRepeated = 1;
            var htRepeated = new Dictionary<char, int>();
            var iMiddle = 0;
            var iMiddleEx = 1;
            var consecutiveMode = 0;
            var iConsecutiveUpper = 0;
            var iConsecutiveLower = 0;
            var iConsecutiveDigit = 0;
            const string sAlphas = "abcdefghijklmnopqrstuvwxyz";
            const string sNumerics = "01234567890";
            var nSeqAlpha = 0;
            var nSeqNumber = 0;

            // Create data table to store results
            createMemoryTable();
            var drScore = dtDetails[0];


            if (string.IsNullOrEmpty(pwd))
            {
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Empty,"",1,0,1000);
                drScore.Bonus = 0;
                drScore.Malus = 1000;
                drScore.Rate = AppResources.PasswordChecker_CheckPasswordWithDetails_Rejected;
                return;
            }

            // Scan password
            foreach (var ch in pwd.ToCharArray())
            {
                // Count digits
                if (char.IsDigit(ch))
                {
                    iDigit++;

                    if (consecutiveMode == 3)
                        iConsecutiveDigit++;
                    consecutiveMode = 3;
                }

                // Count uppercase characters
                if (char.IsUpper(ch))
                {
                    iUpperCase++;
                    if (consecutiveMode == 1)
                        iConsecutiveUpper++;
                    consecutiveMode = 1;
                }

                // Count lowercase characters
                if (char.IsLower(ch))
                {
                    iLowerCase++;
                    if (consecutiveMode == 2)
                        iConsecutiveLower++;
                    consecutiveMode = 2;
                }

                // Count symbols
                if (char.IsSymbol(ch) || char.IsPunctuation(ch))
                {
                    iSymbol++;
                    consecutiveMode = 0;
                }

                // Count repeated letters 
                if (char.IsLetter(ch))
                {
                    if (htRepeated.ContainsKey(char.ToLower(ch))) iRepeated++;
                    else htRepeated.Add(char.ToLower(ch), 0);

                    if (iMiddleEx > 1)
                        iMiddle = iMiddleEx - 1;
                }

                if (iUpperCase > 0 || iLowerCase > 0)
                {
                    if (char.IsDigit(ch) || char.IsSymbol(ch))
                        iMiddleEx++;
                }
            }

            // Check for sequential alpha string patterns (forward and reverse) 
            for (var s = 0; s < 23; s++)
            {
                var sFwd = sAlphas.Substring(s, 3);
                var sRev = strReverse(sFwd.ToCharArray());
                if (pwd.ToLower().IndexOf(sFwd, StringComparison.Ordinal) == -1 && pwd.ToLower().IndexOf(sRev, StringComparison.Ordinal) == -1) continue;
                nSeqAlpha++;
            }

            // Check for sequential numeric string patterns (forward and reverse)
            for (var s = 0; s < 8; s++)
            {
                var sFwd = sNumerics.Substring(s, 3);
                var sRev = strReverse(sFwd.ToCharArray());
                if (pwd.ToLower().IndexOf(sFwd, StringComparison.Ordinal) == -1 && pwd.ToLower().IndexOf(sRev, StringComparison.Ordinal) == -1) continue;
                nSeqNumber++;
            }

            // Score += 4 * Password Length
            var nScore = 4 * pwd.Length;
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Password_Length, "(n*4)", pwd.Length, pwd.Length * 4, 0);

            // if we have uppercase letters Score +=(number of uppercase letters *2)
            if (iUpperCase > 0)
            {
                nScore += ((pwd.Length - iUpperCase) * 2);
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Uppercase_Letters, "+((len-n)*2)", iUpperCase, ((pwd.Length - iUpperCase) * 2), 0);
            }
            else
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Uppercase_Letters, "+((len-n)*2)", iUpperCase, 0, 0);

            // if we have lowercase letters Score +=(number of lowercase letters *2)
            if (iLowerCase > 0)
            {
                nScore += ((pwd.Length - iLowerCase) * 2);
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Lowercase_Letters, "+((len-n)*2)", iLowerCase, ((pwd.Length - iLowerCase) * 2), 0);
            }
            else
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Lowercase_Letters, "+((len-n)*2)", iLowerCase, 0, 0);


            // Score += (Number of digits *4)
            nScore += (iDigit * 4);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Numbers, "+(n*4)", iDigit, (iDigit * 4), 0);

            // Score += (Number of Symbols * 6)
            nScore += (iSymbol * 6);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Symbols, "+(n*6)", iSymbol, (iSymbol * 6), 0);

            // Score += (Number of digits or symbols in middle of password *2)
            nScore += (iMiddle * 2);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Middle_Numbers_or_Symbols, "+(n*2)", iMiddle, (iMiddle * 2), 0);

            //requirments
            var requirments = 0;
            if (pwd.Length >= 8) requirments++;     // Min password length
            if (iUpperCase > 0) requirments++;      // Uppercase letters
            if (iLowerCase > 0) requirments++;      // Lowercase letters
            if (iDigit > 0) requirments++;          // Digits
            if (iSymbol > 0) requirments++;         // Symbols

            if (pwd.Length>=8)
            {
                nScore += pwd.Length*2;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Requirement__Length_8,"+(n*2)",pwd.Length,pwd.Length*2,0);
            }

            if (iUpperCase>0)
            {
                nScore += iUpperCase*2;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Requirement__Uppercase,"+(n)",iUpperCase,iUpperCase,0);
            }

            if (iLowerCase>0)
            {
                nScore += iLowerCase;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Requirement__Lowercase,"+(n)",iLowerCase,iLowerCase,0);
            }

            if (iDigit>0)
            {
                nScore += iDigit;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Requirement__Digit,"+(n)",iDigit,iDigit,0);
            }

            if (iSymbol>0)
            {
                nScore += iSymbol*2;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Requirement__Symbol,"+(2n)",iSymbol,iSymbol*2,0);
            }

            // If we have more than 3 requirments then
            if (requirments > 3)
            {
                nScore += (requirments * 2);
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails__3_or_more_requirments, "+(n*2)", requirments, (requirments * 2), 0);
            }
            else
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Less_than_3_requirments, "-(5-n)*2", requirments, 0, (5-requirments)*2);

            //
            // Deductions
            //

            // if all letter are the same
            if (new string(pwd[0],pwd.Length)==pwd)
            {
                nMalus += pwd.Length*pwd.Length;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Same_symbol,"(n*n)",pwd.Length,0,pwd.Length*pwd.Length);
            }

            // If only letters then score -=  password length
            if (iDigit == 0 && iSymbol == 0)
            {
                nMalus += pwd.Length;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Letters_only, "-n", pwd.Length, 0, pwd.Length);
            }
            else
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Letters_only, "-n", 0, 0, 0);

            // If only digits then score -=  password length
            if (iDigit == pwd.Length)
            {
                nMalus += pwd.Length;
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Numbers_only, "-n", pwd.Length, 0, pwd.Length);
            }
            else
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Numbers_only, "-n", 0, 0, 0);

            // If repeated letters used then score -= (iRepeated * (iRepeated - 1));
            if (iRepeated > 1)
            {
                nMalus += (iRepeated *4);
                addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Repeat_Characters__Case_Insensitive_, "-(n*4)", iRepeated, 0, iRepeated * 4);
            }

            // If Consecutive uppercase letters then score -= (iConsecutiveUpper * 2);
            nMalus += (iConsecutiveUpper * 2);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Consecutive_Uppercase_Letters, "-(n*2)", iConsecutiveUpper, 0, iConsecutiveUpper * 2);

            // If Consecutive lowercase letters then score -= (iConsecutiveUpper * 2);
            nMalus += (iConsecutiveLower * 2);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Consecutive_Lowercase_Letters, "-(n*2)", iConsecutiveLower, 0, iConsecutiveLower * 2);

            // If Consecutive digits used then score -= (iConsecutiveDigits* 2);
            nMalus += (iConsecutiveDigit * 2);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Consecutive_Numbers, "-(n*2)", iConsecutiveDigit, 0, iConsecutiveDigit * 2);

            // If password contains sequence of letters then score -= (nSeqAlpha * 3)
            nMalus += (nSeqAlpha * 3);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Sequential_Letters__3__, "-(n*3)", nSeqAlpha, 0, nSeqAlpha * 3);

            // If password contains sequence of digits then score -= (nSeqNumber * 3)
            nMalus += (nSeqNumber * 3);
            addDetailsRow(AppResources.PasswordChecker_CheckPasswordWithDetails_Sequential_Numbers__3__, "-(n*3)", nSeqNumber, 0, nSeqNumber * 3);

            var fScore = nScore - nMalus;
            /* Determine complexity based on overall score */
            //if (fScore > 100) { fScore = 100; } else if (fScore < 0) { fScore = 0; }
            if (fScore < 25) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Very_Weak; }
            else if (fScore >= 25 && fScore < 50) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Weak; }
            else if (fScore >= 50 && fScore < 60) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Good; }
            else if (fScore >= 60 && fScore < 80) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Strong; }
            else if (fScore >= 80 && fScore < 90) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Very_Strong; }
            else if (fScore >= 90) { sComplexity = AppResources.PasswordChecker_CheckPasswordWithDetails_Ultra_Strong; }
            
            

            // Store score and complexity
            drScore.Bonus = nScore;
            drScore.Malus = nMalus;
            drScore.Rate = sComplexity;

            
        }

        /// <summary>
        /// Create datatable for results
        /// </summary>
        private void createMemoryTable()
        {
            dtDetails = new List<AnalysisItem>();
            addDetailsRow(AppResources.PasswordChecker_createMemoryTable_Score, "", 0, 0, 0);
        }

        /// <summary>
        /// Helper method to add row into memory table
        /// </summary>
        /// <param name="description"></param>
        /// <param name="rate"></param>
        /// <param name="count"></param>
        /// <param name="bonus"></param>
        /// <param name="malus"></param>
        /// <returns></returns>
        private void addDetailsRow(string description, string rate, int count, int bonus, int malus)
        {
            var dr = new AnalysisItem
                         {
                             Description = description ?? "",
                             Rate = rate ?? "",
                             Count = count,
                             Bonus = bonus,
                             Malus = malus
                         };
            dtDetails.Add(dr);
            return;
        }

        /// <summary>
        /// Helper string function to reverse string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string strReverse(IEnumerable<char> str)
        {
            return str.Aggregate("", (s, t) => t + s);
        }
    }

    /// <summary>
    /// Analysis result for one rule
    /// </summary>
    public class AnalysisItem
    {
        public string Description { get; set; }
        public string Rate { get; set; }
        public int Count { get; set; }
        public int Bonus { get; set; }
        public int Malus { get; set; }
        public int Total => Bonus - Malus;
        public string Detail => $"{Rate}   n={Count}";
        public bool IsBonus => Bonus > Malus;
        public bool IsMalus => Malus > Bonus;
        public bool IsNull => Bonus == Malus;
    }
}

