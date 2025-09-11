using Avalonia.Controls;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class MainWindow : Window
{
    public MainWindow(IMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
