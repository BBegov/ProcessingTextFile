using System.Text.RegularExpressions;

namespace TFPLibrary;

public static class TextProcessor
{
    public static string[] SplitTextBySpace(string text)
    {
        return text.ReduceWhiteSpacesToSingle().Split(" ");
    }

    private static string ReduceWhiteSpacesToSingle(this string text)
    {
        return string.IsNullOrEmpty(text)
            ? string.Empty
            : Regex.Replace(text, @"\s+", " ").Trim();
    }

    public static Dictionary<string, int> CountWordsOccurrences(string[] words)
    {
        var wordsAndCounts = new Dictionary<string, int>();

        foreach (var word in words)
        {
            wordsAndCounts[word] = wordsAndCounts.ContainsKey(word)
                ? wordsAndCounts[word] + 1
                : 1;
        }

        return wordsAndCounts;
    }

    public static (string, int)[] ConvertToDescendingArray(IDictionary<string, int> wordsAndCounts)
    {
        return wordsAndCounts
            .Select(kvp => (kvp.Key, kvp.Value))
            .OrderByDescending(kvp => kvp.Value)
            .ToArray();
    }
}
