namespace TFPLibrary.Models;

public class CountingProgressModel : IReportProgress
{
    public Dictionary<string, int> WordsAndCounts { get; set; } = new();
    public HashSet<string> UniqueWordsSet { get; set; } = new();

    public int ReportProgress() => WordsAndCounts.Count == 0
        ? 0
        : UniqueWordsSet.Count * 100 / WordsAndCounts.Count;
}