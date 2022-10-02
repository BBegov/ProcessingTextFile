using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TFPDesktopUI.Models;
using TFPDesktopUI.Service.Extensions;
using TFPLibrary;

namespace TFPDesktopUI.ViewModels;

[ObservableObject]
public partial class MainWindowViewModel
{
    [ObservableProperty]
    private List<TextFileResult> _textFileResults = new();

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string? _infoMessage;

    [ObservableProperty]
    private int _progressbarValue;

    [ObservableProperty]
    private string? _percentageComplete;

    [RelayCommand]
    private void BrowseFile()
    {
        FilePath = string.Empty;
        ResetProperties();

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        if (dialog.ShowDialog() == false) return;

        FilePath = dialog.FileName;
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task Analyze(CancellationToken ct)
    {
        ResetProperties();
  
        var progress = new Progress<int>();
        progress.ProgressChanged += ReportProgress;

        if (FilePath == string.Empty)
        {
            InfoMessage = "Browse a file first!";
            return;
        }

        try
        {
            await ParseSelectedFile(progress, ct);
        }
        catch (OperationCanceledException)
        {
            InfoMessage += " Operation was canceled.";
        }
        catch (IOException exception)
        {
            InfoMessage =
                "An exception occurred:\n" +
                $"Error code: {exception.HResult & 0x0000FFFF}\n" +
                $"Message: {exception.Message}";
        }
    }

    private void ReportProgress(object? sender, int e)
    {
        ProgressbarValue = e;
        PercentageComplete = $"{e}%";
    }

    private void ResetProperties()
    {
        InfoMessage = string.Empty;
        ProgressbarValue = 0;
        PercentageComplete = string.Empty;
        TextFileResults = new List<TextFileResult>();
    }

    private async Task ParseSelectedFile(IProgress<int> progress, CancellationToken ct)
    {
        var fileLinesCount = FileHandler.CountNumberOfLinesInFile(FilePath);

        if (fileLinesCount == 0)
        {
            InfoMessage = "File is empty.";
            return;
        }

        var fileContent = await ReadFile(progress, ct);
        var singleWords = await SeparateText(fileContent, progress, ct);
        var wordsWithOccurrences = await CountUniqueWords(singleWords, progress, ct);
        var descendingResults = await PrepareResult(wordsWithOccurrences, progress, ct);

        TextFileResults = descendingResults.ToTextFileResults();
    }

    private async Task<string> ReadFile(IProgress<int> progress, CancellationToken ct)
    {
        InfoMessage = "1. Reading file ... ";
        var fileContent = await FileHandler.ReadFileByLinesAsync(FilePath, progress, ct);
        InfoMessage += "Done";

        return fileContent;
    }

    private async Task<string[]> SeparateText(string fileContent, IProgress<int> progress, CancellationToken ct)
    {
        InfoMessage += "\n2. Separating to single words ... ";
        var singleWords = await Task.Run(() => TextProcessor.SplitTextBySpace(fileContent), ct);
        InfoMessage += "Done";
        progress.Report(93);

        return singleWords;
    }

    private async Task<Dictionary<string, int>> CountUniqueWords(string[] singleWords, IProgress<int> progress, CancellationToken ct)
    {
        InfoMessage += "\n3. Counting unique words ... ";
        var wordsWithOccurrences = await Task.Run(() => TextProcessor.CountWordsOccurrences(singleWords), ct);
        InfoMessage += "Done";
        progress.Report(98);

        return wordsWithOccurrences;
    }

    private async Task<(string, int)[]> PrepareResult(IDictionary<string, int> wordsWithOccurrences, IProgress<int> progress, CancellationToken ct)
    {
        InfoMessage += "\n4. Preparing results in descending order ... ";
        var descendingResults = await Task.Run(() => TextProcessor.ConvertToDescendingArray(wordsWithOccurrences), ct);
        InfoMessage += "Done";
        progress.Report(100);

        return descendingResults;
    }
}
