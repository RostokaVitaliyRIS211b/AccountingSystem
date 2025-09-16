using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class RolesWindow : Window
{
    public RolesWindow(IRolesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}