using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class AuthorizationWindow : Window
{
    public AuthorizationWindow(IAuthorizationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}