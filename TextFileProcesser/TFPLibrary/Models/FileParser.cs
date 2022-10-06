using TFPLibrary.Extensions;

namespace TFPLibrary.Models;

internal class FileParser : ParserBase
{
    public FileParser(ProgressReportingModel progressModel, IProgress<int> progress, CancellationToken ct) 
        : base(progressModel, progress, ct)
    {
    }

    public async Task<(string, int)[]> ParseFileContentAsync(string[] textLines)
    {
        await SplitTextBySpaceAsync(textLines);
        await CountWordsAndOccurrences();
        await ConvertToList();
        return await OrderListByDescending();
    }

    private async Task SplitTextBySpaceAsync(IEnumerable<string> textLines)
    {
        ProcessingStep = ProcessingSteps.SplittingStep;
        ProgressModel.Lines = textLines.ToList();

        var splittingTask = Task.Run(() =>
        {
            ProgressModel.Words.AddRange(ProgressModel.Lines.SelectMany(line =>
            {
                ProgressModel.ProgressedLines++;
                return line.ReduceWhiteSpacesToSingle().Split(" ");
            }));
        }, Ct);

        var reportingTask = ReportProgressAsync();

        await Task.WhenAll(splittingTask, reportingTask);
        ProgressModel.UniqueWordsSet = ProgressModel.Words.ToHashSet();
    }

    private async Task CountWordsAndOccurrences()
    {
        ProcessingStep = ProcessingSteps.CountingStep;
        var countTask = Task.Run(() =>
        {
            foreach (var word in ProgressModel.Words)
            {
                ProgressModel.WordsAndCounts[word] = ProgressModel.WordsAndCounts.ContainsKey(word)
                    ? ProgressModel.WordsAndCounts[word] + 1
                    : 1;
            }
        }, Ct);

        var reportingTask = ReportProgressAsync();

        await Task.WhenAll(countTask, reportingTask);
    }

    private async Task ConvertToList()
    {
        ProcessingStep = ProcessingSteps.ConvertingStep;
        var convertingTask = Task.Run(() =>
        {
            foreach (var (word, count) in ProgressModel.WordsAndCounts)
            {
                ProgressModel.UnorderedResultList.Add((word, count));
            }
        }, Ct);

        var reportingConvertTask = ReportProgressAsync();

        await Task.WhenAll(convertingTask, reportingConvertTask);
    }

    private async Task<(string, int)[]> OrderListByDescending()
    {
        ProcessingStep = ProcessingSteps.OrderingStep;
        var orderedList = new List<(string, int)>();

        var orderingTask = Task.Run(() =>
        {
            orderedList = ProgressModel.UnorderedResultList
                .OrderByDescending(wordAndCount =>
                {
                    ProgressModel.CheckedWords.Add(wordAndCount.Item1);
                    return wordAndCount.Item2;
                })
                .ToList();
        }, Ct);

        var reportingOrderingTask = ReportProgressAsync();

        await Task.WhenAll(orderingTask, reportingOrderingTask);
        return orderedList.ToArray();
    }
}
