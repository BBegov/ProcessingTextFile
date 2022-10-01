namespace TFPLibrary;

public interface IFileHandler
{
    Task<string> ReadFileByLinesAsync(string filePath, IProgress<int> progress, CancellationToken ct);
    int CountNumberOfLinesInFile(string filePath);
}