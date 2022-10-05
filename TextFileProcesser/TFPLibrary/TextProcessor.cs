using System.Text.RegularExpressions;
using TFPLibrary.Models;

namespace TFPLibrary;

internal static class TextProcessor
{
    internal static async Task ReportProgressAsync(IReportProgress progressModel, 
        IProgress<int> progress, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
        progress.Report(progressModel.ReportProgress());
    }

    internal static async Task<List<string>> SplitTextBySpace(string[] textLines, 
        IProgress<int> progress, CancellationToken ct)
    {
        var words = new List<string>();
        var progressModel = new SplittingProgressModel
        {
            Lines = textLines.ToList()
        };

        var splittingTask = Task.Run(() =>
        {
            words.AddRange(textLines.SelectMany(line =>
            {
                progressModel.ProgressedLines.Add(line);
                return line.ReduceWhiteSpacesToSingle().Split(" ");
            }));
        }, ct);

        var reportingTask = ReportProgressAsync(progressModel, progress, ct);

        await Task.WhenAll(splittingTask, reportingTask);
        return words;
    }

    internal static string ReduceWhiteSpacesToSingle(this string text)
    {
        return string.IsNullOrEmpty(text)
            ? string.Empty
            : Regex.Replace(text, @"\s+", " ").Trim();
    }

    internal static async Task<Dictionary<string, int>> CountingProgressModel(string[] words, 
        IProgress<int> progress, CancellationToken ct)
    {
        var progressModel = new CountingProgressModel
        {
            UniqueWordsSet = words.ToHashSet()
        };

        var countTask = Task.Run(() =>
        {
            foreach (var word in words)
            {
                progressModel.WordsAndCounts[word] = progressModel.WordsAndCounts.ContainsKey(word)
                    ? progressModel.WordsAndCounts[word] + 1
                    : 1;
            }
        }, ct);

        var reportingTask = ReportProgressAsync(progressModel, progress, ct);

        await Task.WhenAll(countTask, reportingTask);
        return progressModel.WordsAndCounts;
    }

    internal static async Task<List<(string, int)>> ConvertToList(Dictionary<string, int> wordsAndCounts, 
        IProgress<int> progress, CancellationToken ct)
    {
        var convertingProgressModel = new ConvertingProgressModel
        {
            WordsAndCounts = wordsAndCounts
        };

        var convertingTask = Task.Run(() =>
        {
            foreach (var (word, count) in convertingProgressModel.WordsAndCounts)
            {
                convertingProgressModel.UnorderedResultList.Add((word, count));
            }
        }, ct);

        var reportingConvertTask = ReportProgressAsync(convertingProgressModel, progress, ct);

        await Task.WhenAll(convertingTask, reportingConvertTask);
        return convertingProgressModel.UnorderedResultList;
    }

    internal static async Task<List<(string, int)>> OrderListByDescending(List<(string, int)> unorderedList, 
        IProgress<int> progress, CancellationToken ct)
    {
        var orderingProgressModel = new OrderingProgressModel
        {
            UnOrderedResultList = unorderedList
        };

        var orderedList = new List<(string, int)>();

        var orderingTask = Task.Run(() =>
        {
            orderedList = unorderedList
                .OrderByDescending(wordAndCount =>
                {
                    orderingProgressModel.CheckedWords.Add(wordAndCount.Item1);
                    return wordAndCount.Item2;
                })
                .ToList();
        }, ct);

        var reportingOrderingTask = ReportProgressAsync(orderingProgressModel, progress, ct);

        await Task.WhenAll(orderingTask, reportingOrderingTask);
        return orderedList;
    }
}