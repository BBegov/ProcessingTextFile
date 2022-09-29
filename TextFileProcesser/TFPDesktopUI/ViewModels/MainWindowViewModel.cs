using System.Collections.Generic;
using System.IO;
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
    
    [ObservableProperty]
    private List<TextFileResult> _textFileResults = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BrowseFileCommand))]
    private string _filePath = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private string _infoMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AnalyzeCommand))]
    private int _percentageComplete;

    public MainWindowViewModel(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        _fileHandler = fileHandler;
        _textProcessor = textProcessor;
    }

    private bool CanAnalyze() 
        => FilePath != string.Empty;

    
    [RelayCommand]
    private void Cancel()
    {
    }

    [RelayCommand]
    private async Task Analyze()
    {
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

            //Reading the file
            InfoMessage = "1. Reading file...";
            var fileContent = await _fileHandler.ReadFileByLines(FilePath);
            PercentageComplete = 50;
            InfoMessage += "Done";

            //Processing the file
            InfoMessage += "\nProcessing file...";
            await Task.Run(() => ProcessFileContent(fileContent));
            PercentageComplete = 50;
            InfoMessage += "Done";
        }
        catch (IOException exception)
        {
            InfoMessage = $"An exception occurred:\nError code: {exception.HResult & 0x0000FFFF}\nMessage: {exception.Message}";
        }
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

    private void ProcessFileContent(string[] fileContent)
    {
        var singleWords = _textProcessor.SeparateTextToSingleWords(fileContent);
        var wordsWithOccurrences = _textProcessor.CountWordsOccurrences(singleWords);

        TextFileResults = wordsWithOccurrences.ToTextFileResults();
    }
}
