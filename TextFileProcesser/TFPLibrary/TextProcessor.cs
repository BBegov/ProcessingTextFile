using System.Text.RegularExpressions;

namespace TFPLibrary;

public class TextProcessor : ITextProcessor
{
    public string[] SeparateTextToSingleWords(string text, string delimiter = " ")
    {
        return CleanText(text)
            .Split(delimiter)
            .ToArray()[1..];
    }

    private static string CleanText(string text)
    {
        if (text.Length == 0) return string.Empty;

        var wordsWithSingleSpaces = Regex.Replace(text, @"\s+", " ");
        var cleanedText = (text[0] != ' ' ? " " : "") + wordsWithSingleSpaces;
        
        return cleanedText;
    }

    public (string, int)[] CountWordsOccurrences(string[] words)
    {
        var wordsAndCounts = new Dictionary<string, int>();

        foreach (var word in words.Select(word => word.Trim()))
        {
            wordsAndCounts[word] = wordsAndCounts.ContainsKey(word) ? wordsAndCounts[word] + 1 : 1;
        }

        return wordsAndCounts
            .Select(kvp => (kvp.Key, kvp.Value))
            .OrderByDescending(kvp => kvp.Value)
            .ToArray();
    }
}

