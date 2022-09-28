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

        if (FilePath.Text == string.Empty)
        {
            ShowMessage("Browse a file first!");
            return;
        }

        try
        {
            var fileContent = await _fileHandler.ReadFileByLines(FilePath.Text);

            if (fileContent.Length == 0)
            {
                ShowMessage("File is empty.");
                return;
            }

            var wordsWithOccurrences = await Task.Run(() => ProcessFileContent(fileContent));

            Task.Run(() => PopulateResultList(wordsWithOccurrences)).Wait();
        }
        catch (IOException exception)
        {
            ShowMessage($"An exception occurred:\nError code: {exception.HResult & 0x0000FFFF}\nMessage: {exception.Message}");
            return;
        }

        LoadResult();
    }

    private void LoadResult()
    {
        ResultList.ItemsSource = TextFileResults;
        ResultList.Visibility = Visibility.Visible;
        ShowMessage(FilePath.Text[(FilePath.Text.LastIndexOf(@"\", StringComparison.Ordinal) + 1)..]);
    }

    private List<TextFileResult> ProcessFileContent(string[] fileContent)
    {
        var singleWords = _textProcessor.SeparateToSingleWords(fileContent);
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
        CollapseWindowItems();
        ClearResultCollection();
    }

    private void CollapseWindowItems()
    {
        ProgressBar.Visibility = Visibility.Collapsed;
        ProgressBarLabel.Visibility = Visibility.Collapsed;
        InfoMessage.Visibility = Visibility.Collapsed;
        ResultList.Visibility = Visibility.Collapsed;
        Cancel.Visibility = Visibility.Collapsed;
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
}
