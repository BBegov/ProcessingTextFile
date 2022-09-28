using System.Windows;
using TFPDesktopUI.Models;
using TFPLibrary;

namespace TFPDesktopUI;

public partial class MainWindow : Window
{
    private readonly IFileHandler _fileHandler;
    private readonly ITextProcessor _textProcessor;

    public MainWindow(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        InitializeComponent();
        _fileHandler = fileHandler;
        _textProcessor = textProcessor;
        ResetApp();
    }

    private void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        ResetApp();

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

    private void Analyze_Click(object sender, RoutedEventArgs e)
    {
        ResetApp();

        if (FilePath.Text == string.Empty)
        {
            ShowMessage("Browse a file first!");
            return;
        }

        var fileContent = _fileHandler.ReadFileByLines(FilePath.Text);
        
        if (fileContent.Length == 0)
        {
            ShowMessage("File is empty.");
            return;
        }

        var singleWords = _textProcessor.SeparateToSingleWords(fileContent);
        var wordsWithOccurrences = _textProcessor.CountWordsOccurrences(singleWords);
        
        foreach (var wordWithOccurrence in wordsWithOccurrences)
        {
            var item = new TextFileResult { Word = wordWithOccurrence.Item1, Occurrence = wordWithOccurrence.Item2 };
            ResultList.Items.Add(item);
        }

        ResultList.Visibility = Visibility.Visible;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void ResetApp()
    {
        CollapseWindowItems();
        ClearResultList();
    }

    private void CollapseWindowItems()
    {
        ProgressBar.Visibility = Visibility.Collapsed;
        ProgressBarLabel.Visibility = Visibility.Collapsed;
        ResultMessage.Visibility = Visibility.Collapsed;
        ResultList.Visibility = Visibility.Collapsed;
        Cancel.Visibility = Visibility.Collapsed;
    }

    private void ClearResultList()
    {
        ResultList.Items.Clear();
    }

    private void ShowMessage(string message)
    {
        ResultMessage.Text = message;
        ResultMessage.Visibility = Visibility.Visible;
    }
}
