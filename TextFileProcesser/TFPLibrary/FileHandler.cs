namespace TFPLibrary;

public class FileHandler : IFileHandler
{
    public async Task<string[]> ReadFileByLines(string path)
    {
        return await File.ReadAllLinesAsync(path);
    }
}
