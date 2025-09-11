using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Helpers
{
    internal static class ColorHelper
    {
        public static void Current_ActualThemeVariantChanged(object? sender, System.EventArgs e)
        {
            if(Application.Current != null)
            {
                if (Application.Current.ActualThemeVariant == ThemeVariant.Dark)
                {
                    Application.Current.Resources["LightGrayBrush"] = new SolidColorBrush(Colors.Gray);
                    Application.Current.Resources["WhiteSmokeBrush"] = new SolidColorBrush(Colors.DimGray);
                    Application.Current.Resources["RedBrush"] = new SolidColorBrush(Colors.Gold);
                    Application.Current.Resources["BlueBrush"] = new SolidColorBrush(Colors.LightSkyBlue);
                    Application.Current.Resources["AliceBlueBrush"] = new SolidColorBrush(Colors.SteelBlue);
                }
                else if(Application.Current.ActualThemeVariant == ThemeVariant.Light)
                {
                    Application.Current.Resources["LightGrayBrush"] = new SolidColorBrush(Colors.LightGray);
                    Application.Current.Resources["WhiteSmokeBrush"] = new SolidColorBrush(Colors.WhiteSmoke);
                    Application.Current.Resources["RedBrush"] = new SolidColorBrush(Colors.Red);
                    Application.Current.Resources["BlueBrush"] = new SolidColorBrush(Colors.Blue);
                    Application.Current.Resources["AliceBlueBrush"] = new SolidColorBrush(Colors.AliceBlue);
                }
            }
        }
    }
}
