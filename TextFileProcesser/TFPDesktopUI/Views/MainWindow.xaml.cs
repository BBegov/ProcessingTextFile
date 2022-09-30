using System.Windows;
using TFPDesktopUI.ViewModels;
using TFPLibrary;

namespace TFPDesktopUI.Views;

public partial class MainWindow : Window
{
    public MainWindow(IFileHandler fileHandler, ITextProcessor textProcessor)
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(fileHandler, textProcessor);
    }
}
