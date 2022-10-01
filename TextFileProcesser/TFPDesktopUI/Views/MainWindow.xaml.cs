using TFPDesktopUI.ViewModels;

namespace TFPDesktopUI.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
