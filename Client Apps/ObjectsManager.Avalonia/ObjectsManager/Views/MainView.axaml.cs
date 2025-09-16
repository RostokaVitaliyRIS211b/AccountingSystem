using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace ObjectsManager.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void MenuItem_Theme_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(sender is MenuItem item && item.Tag is ThemeVariant variant && Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = variant;
        }
    }
}
