using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Views;

public partial class ProducerSelectorView : UserControl
{
    public ProducerSelectorView()
    {
        InitializeComponent();
    }

    private void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is IProducerViewModel viewModel && viewModel.SelectedProducer is not null)
        {
            viewModel.Win?.Close();
        }
    }
}