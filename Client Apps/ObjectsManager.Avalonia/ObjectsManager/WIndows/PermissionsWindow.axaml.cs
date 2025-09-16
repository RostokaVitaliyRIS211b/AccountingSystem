using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class PermissionsWindow : Window
{
    public PermissionsWindow(IPermissionsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}