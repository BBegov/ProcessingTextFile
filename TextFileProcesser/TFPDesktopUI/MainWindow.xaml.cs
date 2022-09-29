using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using TFPDesktopUI.Models;
using TFPLibrary;
using TFPDesktopUI.Service.Extensions;

namespace TFPDesktopUI;

public partial class MainWindow : Window
{
    private readonly IFileHandler _fileHandler;
    private readonly ITextProcessor _textProcessor;
    public ObservableCollection<TextFileResult> TextFileResults { get; private set; } = new();

    public MainWindow(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        InitializeComponent();
        DataContext = this;
        _fileHandler = fileHandler;
        _textProcessor = textProcessor;
        ResetApp();
    }

    private void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        ResetApp();
        FilePath.Text = string.Empty;

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            FileName = "Document",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        var isFileChosen = dialog.ShowDialog();

        if (isFileChosen == false) return;

        FilePath.Text = dialog.FileName;
    }

    private async void Analyze_Click(object sender, RoutedEventArgs e)
    {
        ResetApp();
        EnableMainButtons(false);

        var path = FilePath.Text;

        if (path == string.Empty)
        {
            ShowMessage("Browse a file first!");
            return;
        }

        try
        {
            var fileLinesCount = _fileHandler.CountNumberOfLinesInFile(path);

            if (fileLinesCount == 0)
            {
                ShowMessage("File is empty.");
                EnableMainButtons(true);
                return;
            }

            // Reading the file
            ShowProgressBarElements();
            ShowMessage("1. Reading file...");
            var fileContent = await _fileHandler.ReadFileByLines(path);
            ReportProgress((1 * 100) / 3);

            // Processing the file
            AddToMessage("\nProcessing file...");
            var wordsWithOccurrences = await Task.Run(() => ProcessFileContent(fileContent));
            ReportProgress((2 * 100) / 3);
            
            // Populating results
            AddToMessage("\nPreparing result...");
            Task.Run(() => PopulateResultList(wordsWithOccurrences)).Wait();
            ReportProgress((3 * 100) / 3);
        }
        catch (IOException exception)
        {
            ShowMessage($"An exception occurred:\nError code: {exception.HResult & 0x0000FFFF}\nMessage: {exception.Message}");
            return;
        }

        LoadResult();
        ShowResult();
    }

    private void ReportProgress(int percentageComplete)
    {
        if (ProgressBar.Visibility != Visibility.Visible)
        {
            ProgressBar.Visibility = Visibility.Visible;
        }

        AddToMessage("Done");

        ProgressBar.Value = percentageComplete;
    }

    private void LoadResult()
    {
        ResultList.ItemsSource = TextFileResults;
    }

    private void ShowResult()
    {
        CollapseProgressBarElements();
        EnableMainButtons(true);
        ResultList.Visibility = Visibility.Visible;
        ShowMessage(FilePath.Text[(FilePath.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1)..]);
        ProgressBar.Value = 0;
    }

    private List<TextFileResult> ProcessFileContent(string[] fileContent)
    {
        var singleWords = _textProcessor.SeparateTextToSingleWords(fileContent);
        var wordsWithOccurrences = _textProcessor.CountWordsOccurrences(singleWords);

        return wordsWithOccurrences.ToTextFileResults();
    }

    private void PopulateResultList(IEnumerable<TextFileResult> wordsWithOccurrences)
    {
        foreach (var wordWithOccurrence in wordsWithOccurrences)
        {
            TextFileResults.Add(wordWithOccurrence);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void ResetApp()
    {
        EnableMainButtons(true);
        CollapseWindowItems();
        ClearResultCollection();
    }

    private void EnableMainButtons(bool status)
    {
        BrowseFile.IsEnabled = status;
        Analyze.IsEnabled = status;
    }

    private void CollapseWindowItems()
    {
        CollapseProgressBarElements();
        InfoMessage.Visibility = Visibility.Collapsed;
        ResultList.Visibility = Visibility.Collapsed;
    }

    private void CollapseProgressBarElements()
    {
        ProgressBar.Visibility = Visibility.Collapsed;
        ProgressBarLabel.Visibility = Visibility.Collapsed;
        Cancel.Visibility = Visibility.Collapsed;
    }

    private void ShowProgressBarElements()
    {
        ProgressBar.Visibility = Visibility.Visible;
        ProgressBarLabel.Visibility = Visibility.Visible;
        Cancel.Visibility = Visibility.Visible;
    }

    private void ClearResultCollection()
    {
        TextFileResults = new ObservableCollection<TextFileResult>();
    }

    private void ShowMessage(string message)
    {
        InfoMessage.Text = message;
        InfoMessage.Visibility = Visibility.Visible;
    }

    private void AddToMessage(string text)
    {
        InfoMessage.Text += text;
    }
}
