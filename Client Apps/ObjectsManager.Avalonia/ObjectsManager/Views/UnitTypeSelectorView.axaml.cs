using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Views;

public partial class UnitTypeSelectorView : UserControl
{
    public UnitTypeSelectorView()
    {
        InitializeComponent();
    }

    private void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is IUnitTypeSelectorViewModel viewModel && viewModel.SelectedUnit is not null)
        {
            viewModel.Win?.Close();
        }
    }
}