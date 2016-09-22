using System;
using System.Collections.Generic;
using System.Linq;
using MyStash.ResX;

namespace MyStash.Helpers
{
    public static class Utils
    {
        // keys in Properties
        public enum GlobalKeys
        {
            None
        }

        // generic command to VM
        public enum GlobalCommands
        {
            ListviewTapped
        }

        // MVVM messages
        public enum GlobalMessages
        {
            AnimateDoorButton,
            CopyToClipbard,
            SettingsChanged,
            DataModified,
            DataInserted,
            DataDeleted,
            ImportDataCopied
        }

        // disk storage key/value 
        public enum StoredDataKeys
        {
            EncryptedPw
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            if (list == null) return new List<T>();
            return list.OrderBy(a => Guid.NewGuid());
        }

        public static List<char> Shuffle(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? new List<char>() : str.ToCharArray().Shuffle().ToList();
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct 
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }

        public static string NoControlAndTrim(this string value)
        {
            return (new string(value.ToCharArray().Where(c => !char.IsControl(c)).ToArray())).Trim();
        }

        public static bool Contains(this string str, string substring,
                                    StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException(nameof(substring),
                                                AppResources.StringExtensions_Contains_substring_cannot_be_null_);
            if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException(AppResources.StringExtensions_Contains_comp_is_not_a_member_of_StringComparison,
                    nameof(comp));

            return str.IndexOf(substring, comp) >= 0;
        }

    }
}
