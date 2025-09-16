using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class NameSelectorWindow : Window
{
    public NameSelectorWindow(INameSelectorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}