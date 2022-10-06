using TFPLibrary.Models;

namespace TFPLibrary;
public static class TFPService
{
    public static async Task<(string, int)[]> ParseFileContentAsync(string[] textLines, IProgress<int> progress, CancellationToken ct)
    {
        var reportingModel = new ProgressReportingModel();
        var fileContentParser = new FileParser(reportingModel, progress, ct);
        return await fileContentParser.ParseFileContentAsync(textLines);
    }
}
