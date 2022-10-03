namespace TFPLibrary.Models;

public class OrderingProgressModel : IReportProgress
{
    public List<(string, int)> UnOrderedResultList { get; set; } = new();
    public HashSet<string> CheckedWords { get; set; } = new();

    public int ReportProgress() => UnOrderedResultList.Count == 0
        ? 0
        : CheckedWords.Count * 100 / UnOrderedResultList.Count;
}