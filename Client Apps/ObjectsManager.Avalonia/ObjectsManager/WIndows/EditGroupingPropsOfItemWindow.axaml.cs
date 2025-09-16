using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class EditGroupingPropsOfItemWindow : Window
{
    public EditGroupingPropsOfItemWindow(IEditGroupingPropsOfItemViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}