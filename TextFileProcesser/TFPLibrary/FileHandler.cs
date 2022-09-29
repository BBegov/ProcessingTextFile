namespace TFPLibrary;

public class FileHandler : IFileHandler
{
    public async Task<string[]> ReadFileByLines(string path)
    {
        return await File.ReadAllLinesAsync(path);
    }

    public int CountNumberOfLinesInFile(string path)
    {
        return (int) File.ReadLines(path).Count();
    }
}
