using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaComponents.MetaDataView.ViewModels;

namespace AvaloniaComponents.MetaDataView.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if(DataContext is MainViewModel viewModel)
        {
            viewModel.SetTabs(tabCtrl);
        }
    }
}
