namespace TFPLibrary.Models;

public class ConvertingProgressModel : IReportProgress
{
    public Dictionary<string, int> WordsAndCounts { get; set; } = new();
    public List<(string, int)> UnorderedResultList { get; set; } = new();

    public int ReportProgress() => WordsAndCounts.Count == 0
        ? 0
        : UnorderedResultList.Count * 100 / WordsAndCounts.Count;
}