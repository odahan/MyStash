using System;

namespace MyStash.Helpers
{
    /// <summary>
    /// generates random strings (used by ViewModels for design data)
    /// </summary>
    public static class RandomStringGenerator
    {
        private static readonly Random rnd = new Random();
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string Symbols = @"~`!@#$%^&*()-_=+<>?:,./\[]{}|'";

        /// <summary>
        /// returns the next string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="lowerCase">if set to <c>true</c> [lower case].</param>
        /// <param name="upperCase">if set to <c>true</c> [upper case].</param>
        /// <param name="numbers">if set to <c>true</c> [numbers].</param>
        /// <param name="symbols">if set to <c>true</c> [symbols].</param>
        /// <returns></returns>
        public static string GetNewString(int length, bool lowerCase = true, bool upperCase = true, bool numbers = true, bool symbols = true)
        {
            var charArray = new char[length];
            var charPool = string.Empty;

            //Build character pool
            if (lowerCase)
                charPool += Lowercase;

            if (upperCase)
                charPool += Uppercase;

            if (numbers)
                charPool += Numbers;

            if (symbols)
                charPool += Symbols;

            //Build the output character array
            for (var i = 0; i < charArray.Length; i++)
            {
                //Pick a random integer in the character pool
                var index = rnd.Next(0, charPool.Length);

                //Set it to the output character array
                charArray[i] = charPool[index];
            }
            return new string(charArray);
        }

    }
}
