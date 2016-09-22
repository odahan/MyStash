using System.Text;
using System;


namespace MyStash.Helpers
{

    public enum SheetCrypting { None, Algo1, Algo2 }

    public sealed class Encryption
    {
        public static Encryption Instance { get; } = new Encryption();

        private const string DefaultPassword = "Label32_GetColor(25,20,40)!$'Repeat please'NoDataTryAgain";


        private Encryption()
        {
            Password = DefaultPassword;
        }

        public string Password { get; set; }


        public string Encrypt(string text, SheetCrypting algorithm)
        {
            if (algorithm==SheetCrypting.Algo1) return Base64Encode(computeXor(text, Password));
            if (algorithm==SheetCrypting.Algo2) return Base64Encode(text);

            return text;
        }

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The input string to decrypt.</param>
        /// <returns>The decrypted value of the specified input.</returns>
        public string Decrypt(string text, SheetCrypting algorithm)
        {
            switch (algorithm)
            {
                case SheetCrypting.Algo1:
                    var b64Decoded = Base64Decode(text);
                    var r = computeXor(b64Decoded, Password);
                    return r;
                case SheetCrypting.Algo2:
                    return Base64Decode(text);
            }
            return text;
        }

        private static string computeXor(string text, string key)
        {
            var result = new StringBuilder();
            for (var c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));
            return result.ToString();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes,0,base64EncodedBytes.Length);
        }
    }

}

