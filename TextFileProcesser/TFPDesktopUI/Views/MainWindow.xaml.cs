using System.Windows;
using TFPLibrary;
using TFPDesktopUI.ViewModels;

namespace TFPDesktopUI;

public partial class MainWindow : Window
{
    public MainWindow(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(fileHandler, textProcessor);
    }
}
