namespace TFPLibrary;

public class FileHandler : IFileHandler
{
    public string[] ReadFileByLines(string path)
    {
        return File.ReadAllLines(path);
    }
}
