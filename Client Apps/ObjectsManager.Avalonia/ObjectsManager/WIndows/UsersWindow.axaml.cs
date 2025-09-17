using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class UsersWindow : Window
{
    public UsersWindow(IUsersViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }
}