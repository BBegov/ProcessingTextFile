namespace TFPLibrary;

public class FileHandler : IFileHandler
{
    public string ReadFile(string path)
    {
        return System.IO.File.ReadAllText(path);
    }
}
