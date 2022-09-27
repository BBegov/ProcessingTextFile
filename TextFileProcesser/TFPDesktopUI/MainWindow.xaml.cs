using System;
using System.IO;
using System.Windows;
using TFPLibrary;

namespace TFPDesktopUI;

public partial class MainWindow : Window
{
    private readonly IFileHandler _fileHandler;

    public MainWindow(IFileHandler fileHandler)
    {
        InitializeComponent();
        _fileHandler = fileHandler;
    }

    private void BrowseFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            FileName = "Document",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt)|*.txt"
        };

        var result = dialog.ShowDialog();

        if (result != true) return;

        FilePath.Text = dialog.FileName;
        ResultWindow.Text = string.Empty;
    }

    private void Analyze_Click(object sender, RoutedEventArgs e)
    {
        if (FilePath.Text == string.Empty)
        {
            ResultWindow.Text = "No file was chosen";
            return;
        }

        var content = _fileHandler.ReadFile(FilePath.Text);
        
        if (content.Length == 0)
        {
            ResultWindow.Text = "File is empty";
            return;
        }
        
        ResultWindow.Text = content;
    }
}
