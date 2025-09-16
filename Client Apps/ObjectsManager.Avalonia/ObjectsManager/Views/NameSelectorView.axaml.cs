using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Views;

public partial class NameSelectorView : UserControl
{
    public NameSelectorView()
    {
        InitializeComponent();
    }

    private void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if(DataContext is INameSelectorViewModel viewModel && viewModel.SelectedName is not null)
        {
            viewModel.Win?.Close();
        }
    }
}