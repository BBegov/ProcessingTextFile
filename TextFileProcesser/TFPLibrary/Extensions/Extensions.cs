using System.Text.RegularExpressions;

namespace TFPLibrary.Extensions;

internal static class TextProcessor
{
    internal static string ReduceWhiteSpacesToSingle(this string text)
    {
        return string.IsNullOrEmpty(text)
            ? string.Empty
            : Regex.Replace(text, @"\s+", " ").Trim();
    }
}