using System.Text.RegularExpressions;

namespace TFPLibrary;

public class TextProcessor : ITextProcessor
{
    public string[] SeparateTextToSingleWords(string text, string delimiter = " ")
    {
        return CleanText(text)
            .Split(delimiter)
            .ToArray();
    }

    private static string CleanText(string text)
    {
        return text.Length == 0 
            ? string.Empty 
            : Regex
                .Replace(text, @"\s+", " ")
                .Trim();
    }

    public (string, int)[] CountWordsOccurrences(string[] words)
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

