using System.Text;
using System.Text.RegularExpressions;

namespace TFPLibrary;

public class TextProcessor : ITextProcessor
{
    public string[] SeparateTextToSingleWords(string[] text, string delimiter = " ")
    {
        var content = new StringBuilder();

        foreach (var line in text)
        {
            var lineToAppend = CleanText(line);
            content.Append(lineToAppend);
        }
        
        return content
            .ToString()
            .Split(delimiter)
            .ToArray()[1..];
    }

    private static string CleanText(string line)
    {
        if (line.Length == 0) return string.Empty;

        var wordsWithSingleSpaces = Regex.Replace(line, @"\s+", " ");
        var cleanedLine = (line[0] != ' ' ? " " : "") + wordsWithSingleSpaces;
        
        return cleanedLine;
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

