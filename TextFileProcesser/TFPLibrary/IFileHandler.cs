namespace TFPLibrary;

public interface IFileHandler
{
    Task<string[]> ReadFileByLinesAsync(string path);
    int CountNumberOfLinesInFile(string path);
}