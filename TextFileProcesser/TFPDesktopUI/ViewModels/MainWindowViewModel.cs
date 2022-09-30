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

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileHandler _fileHandler;
    private readonly ITextProcessor _textProcessor;

    private CancellationTokenSource? _tokenSource;
    
    [ObservableProperty]
    private List<TextFileResult> _textFileResults = new();

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _infoMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private int _progressbarValue;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private string? _percentageComplete;

    public MainWindowViewModel(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        _fileHandler = fileHandler;
        _textProcessor = textProcessor;
    }

    private bool CanCancel()
        => ProgressbarValue != 0 && ProgressbarValue != 100;
    
    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _tokenSource?.Cancel();
    }

    [RelayCommand]
    private async Task Analyze()
    {
        var progress = new Progress<int>();
        progress.ProgressChanged += ReportProgress;

        _tokenSource = new CancellationTokenSource();
        var token = _tokenSource.Token;

        if (FilePath == string.Empty)
        {
            InfoMessage = "Browse a file first!";
            return;
        }

        try
        {
            var fileLinesCount = _fileHandler.CountNumberOfLinesInFile(FilePath);

            if (fileLinesCount == 0)
            {
                InfoMessage = "File is empty.";
                return;
            }

            await FileParsing(progress, token);
        }
        catch (OperationCanceledException)
        {
            InfoMessage = "File parsing was canceled.";
        }
        catch (IOException exception)
        {
            InfoMessage =
                $"An exception occurred:\nError code: {exception.HResult & 0x0000FFFF}\nMessage: {exception.Message}";
        }
        finally
        {
            _tokenSource.Dispose();
        }
    }

    private void ReportProgress(object? sender, int e)
    {
        ProgressbarValue = e;
        PercentageComplete = $"{e}%";
    }

    [RelayCommand]
    private void BrowseFile()
    {
        FilePath = string.Empty;
        InfoMessage = string.Empty;

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            FileName = "Document",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        var isFileChosen = dialog.ShowDialog();

        if (isFileChosen == false) return;

        FilePath = dialog.FileName;
    }

    private async Task FileParsing(IProgress<int> progress, CancellationToken token)
    {
        //Reading the file
        InfoMessage = "1. Reading file...";
        var fileContent = await _fileHandler.ReadFileByLinesAsync(FilePath, progress, token);
        InfoMessage += "Done";

        token.ThrowIfCancellationRequested();

        //Processing the file
        InfoMessage += "\n2. Processing file...";
        await Task.Run(() => ProcessFileContent(fileContent), token);
        InfoMessage += "Done";
    }

    private void ProcessFileContent(string fileContent)
    {
        var singleWords = _textProcessor.SeparateTextToSingleWords(fileContent);
        var wordsWithOccurrences = _textProcessor.CountWordsOccurrences(singleWords);

        TextFileResults = wordsWithOccurrences.ToTextFileResults();
    }
}
