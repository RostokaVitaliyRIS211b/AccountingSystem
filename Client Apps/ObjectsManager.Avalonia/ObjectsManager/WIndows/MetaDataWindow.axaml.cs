using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using AvaloniaComponents.MetaDataView;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class MetaDataWindow : Window
{
    public MetaDataWindow(IMetaDataViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}