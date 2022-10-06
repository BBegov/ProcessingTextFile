using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        catch (InvalidEnumArgumentException exception)
        {
            InfoMessage = 
                "An exception occurred during file parsing:\n" + 
                $"ˇMessage: {exception.Message}";
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

        InfoMessage = "1. Reading file ... ";
        var fileContent = await FileHandler.ReadFileByLinesAsync(FilePath, progress, ct);
        InfoMessage += "Done";
        InfoMessage += "\n2. Parsing file content ... ";
        var descendingResults = await TFPService.ParseFileContentAsync(fileContent, progress, ct);
        InfoMessage += "Done";
        progress.Report(100);

        TextFileResults = descendingResults.ToTextFileResults();
    }
}
