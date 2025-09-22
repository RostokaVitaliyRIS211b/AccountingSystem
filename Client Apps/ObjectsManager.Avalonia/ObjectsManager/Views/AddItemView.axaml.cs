using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Views;

public partial class AddItemView : UserControl
{
    public AddItemView()
    {
        InitializeComponent();
    }

    private void Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(DataContext is IAddItemViewModel viewModel)
        {
            viewModel.Win?.Close();
        }
    }
}