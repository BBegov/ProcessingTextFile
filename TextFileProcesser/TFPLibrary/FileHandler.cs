namespace TFPLibrary;

public static class FileHandler
{
    public static async Task<string> ReadFileByLinesAsync(string filePath, IProgress<int> progress, CancellationToken ct)
    {
        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(stream);

        var readTask = reader.ReadToEndAsync();

        var progressTask = Task.Run(async () =>
        {
            while (stream.Position < stream.Length)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), ct);
                progress.Report((int) ((stream.Position * 100) / stream.Length * 0.75));
            }
        }, ct);

        await Task.WhenAll(readTask, progressTask);

        return readTask.Result;
    }

    public static int CountNumberOfLinesInFile(string filePath)
    {
        return File.ReadLines(filePath).Count();
    }
}
