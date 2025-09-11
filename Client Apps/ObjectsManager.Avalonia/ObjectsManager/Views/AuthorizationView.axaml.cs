using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;

using ObjectsManager.Helpers;

namespace ObjectsManager.Views;

public partial class AuthorizationView : UserControl
{
    public AuthorizationView()
    {
        InitializeComponent();
    }

    private void Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Parent is Window window)
        {
            window.Close();
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if(Application.Current != null)
        {
            if(Application.Current.ActualThemeVariant == ThemeVariant.Dark)
            {
                Application.Current.Resources["LightGrayBrush"] = new SolidColorBrush(Colors.Gray);
                Application.Current.Resources["WhiteSmokeBrush"] = new SolidColorBrush(Colors.DimGray);
                Application.Current.Resources["RedBrush"] = new SolidColorBrush(Colors.Gold);
                Application.Current.Resources["BlueBrush"] = new SolidColorBrush(Colors.LightSkyBlue);
                Application.Current.Resources["AliceBlueBrush"] = new SolidColorBrush(Colors.SteelBlue);
            }
            Application.Current.ActualThemeVariantChanged += ColorHelper.Current_ActualThemeVariantChanged;
        }
    }

   
}