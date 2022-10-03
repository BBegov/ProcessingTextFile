namespace TFPLibrary.Models;

public class SplittingProgressModel : IReportProgress
{
    public List<string> Lines { get; set; } = new();
    public HashSet<string> ProgressedLines { get; set; } = new();

    public int ReportProgress() => Lines.Count == 0
        ? 0
        : ProgressedLines.Count * 100 / Lines.Count;
}