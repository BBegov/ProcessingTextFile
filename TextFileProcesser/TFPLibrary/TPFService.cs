namespace TFPLibrary;
public class TPFService
{
    public static async Task<string[]> SeparateToWords(string[] textLines,
        IProgress<int> progress, CancellationToken ct)
    {
        var words = await TextProcessor.SplitTextBySpace(textLines, progress, ct);

        return words.ToArray();
    }

    public static async Task<Dictionary<string, int>> CountWordsOccurrences(string[] words,
        IProgress<int> progress, CancellationToken ct)
    {
        var wordsAndCounts = await TextProcessor.CountingProgressModel(words, progress, ct);

        return wordsAndCounts;
    }

    public static async Task<(string, int)[]> ConvertToDescendingArray(Dictionary<string, int> wordsAndCounts,
        IProgress<int> progress, CancellationToken ct)
    {
        var unorderedResultList = await TextProcessor.ConvertToList(wordsAndCounts, progress, ct);
        var orderedResultList = await TextProcessor.OrderListByDescending(unorderedResultList, progress, ct);

        return orderedResultList.ToArray();
    }
}
