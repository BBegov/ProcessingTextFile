namespace TFPLibrary.Models;

public class ProgressReportingModel
{
    public List<string> Lines { get; set; } = new();
    public List<string> Words { get; set; } = new();
    public int ProgressedLines { get; set; }
    public Dictionary<string, int> WordsAndCounts { get; set; } = new();
    public HashSet<string> UniqueWordsSet { get; set; } = new();
    public List<(string, int)> UnorderedResultList { get; set; } = new();
    public HashSet<string> CheckedWords { get; set; } = new();
    
    public int ReportProgress1() => Lines.Count == 0
        ? 0
        : ProgressedLines * 100 / Lines.Count;
    
    public int ReportProgress2() => UniqueWordsSet.Count == 0
        ? 0
        : WordsAndCounts.Count * 100 / UniqueWordsSet.Count;

    public int ReportProgress3() => WordsAndCounts.Count == 0
        ? 0
        : UnorderedResultList.Count * 100 / WordsAndCounts.Count;

    public int ReportProgress4() => UnorderedResultList.Count == 0
        ? 0
        : CheckedWords.Count * 100 / UnorderedResultList.Count;
    
}
