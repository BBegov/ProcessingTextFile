namespace TFPLibrary;

public interface IFileHandler
{
    Task<string[]> ReadFileByLines(string path);
    int CountNumberOfLinesInFile(string path);
}