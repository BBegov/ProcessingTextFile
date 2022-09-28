using System.Text;
using System.Text.RegularExpressions;

namespace TFPLibrary;

public class TextProcessor : ITextProcessor
{
    public string[] SeparateToSingleWords(string[] text, string delimiter = " ")
    {
        var content = new StringBuilder();

        foreach (var line in text)
        {
            if (line.Length < 1) continue;

            if (line[0] != ' ')
            {
                content.Append(' ');
            }

            var wordsWithSingleSpaces = Regex.Replace(line, @"\s+", " ");

            content.Append(wordsWithSingleSpaces);
        }
        
        return content
            .ToString()
            .Split(delimiter)
            .ToArray()[1..];
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

