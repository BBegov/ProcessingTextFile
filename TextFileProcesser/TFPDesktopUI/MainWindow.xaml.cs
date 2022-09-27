using System.Windows;
using TFPLibrary;

namespace TFPDesktopUI;

public partial class MainWindow : Window
{
    private readonly IDataAccess _dataAccess;

    public MainWindow(IDataAccess dataAccess)
    {
        InitializeComponent();
        _dataAccess = dataAccess;
    }
}
