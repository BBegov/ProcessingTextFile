namespace TFPLibrary;

public interface IFileHandler
{
    Task<string[]> ReadFileByLines(string path);
}