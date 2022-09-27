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
        CollapseResults();
    }

    private void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        CollapseResults();

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
        CollapseResults();

        if (FilePath.Text == string.Empty)
        {
            ResultMessage.Text = "Browse a file first!";
            ResultMessage.Visibility = Visibility.Visible;
            return;
        }

        var content = _fileHandler.ReadFileByLines(FilePath.Text);
        
        if (content.Length == 0)
        {
            ResultMessage.Text = "File is empty.";
            ResultMessage.Visibility = Visibility.Visible;
            return;
        }

        const string delimiter = " ";
        var words = _textProcessor.SeparateToSingleWords(content, delimiter);
        var results = _textProcessor.CountWordsOccurrences(words);
        
        foreach (var wordWithOccurrence in results)
        {
            var item = new TextFileResult{ Word = wordWithOccurrence.Item1, Occurrence = wordWithOccurrence.Item2 };
            ResultList.Items.Add(item);
        }

        ResultList.Visibility = Visibility.Visible;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        return;
    }

    private void CollapseResults()
    {
        ProgressBar.Visibility = Visibility.Collapsed;
        ProgressBarLabel.Visibility = Visibility.Collapsed;
        ResultMessage.Visibility = Visibility.Collapsed;
        ResultList.Visibility = Visibility.Collapsed;
        Cancel.Visibility = Visibility.Collapsed;
    }
}
