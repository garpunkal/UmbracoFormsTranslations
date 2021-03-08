using System.Text.RegularExpressions;
using System.Globalization;
using Humanizer;

namespace UmbracoFormsTranslations.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveReturns(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            s = s.Replace("\r", "");
            s = s.Replace("\n", " ");

            return s;
        }

        public static string ReplaceSpacesWithReplacement(this string value, string replacement = "")
             => Regex.Replace(value, @"\s+", replacement);

        public static string RemoveNonAlphaNumericChars(this string value)
            => Regex.Replace(value, @"[^A-Za-z0-9]", "");

        public static string ToDictionaryItemName(this string value, string prefix = "Forms", string suffix = "")
        => $"{prefix}.{value.RemoveReturns().Titleize().ReplaceSpacesWithReplacement().RemoveNonAlphaNumericChars()}{suffix}";
    }
}
