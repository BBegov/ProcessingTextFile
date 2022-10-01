using System.Text.RegularExpressions;

namespace TFPLibrary;

public static class TextProcessor
{
    public static string[] SeparateTextToSingleWords(string text, string delimiter = " ")
    {
        return text
            .CleanText()
            .Split(delimiter)
            .ToArray();
    }

    public static string CleanText(this string text)
    {
        return text.Length == 0 
            ? string.Empty 
            : Regex
                .Replace(text, @"\s+", " ")
                .Trim();
    }

    public static (string, int)[] CountWordsOccurrences(string[] words)
    {
        var wordsAndCounts = new Dictionary<string, int>();

        foreach (var word in words)
        {
            wordsAndCounts[word] = wordsAndCounts.ContainsKey(word) 
                ? wordsAndCounts[word] + 1 
                : 1;
        }

        return wordsAndCounts
            .Select(kvp => (kvp.Key, kvp.Value))
            .OrderByDescending(kvp => kvp.Value)
            .ToArray();
    }
}

