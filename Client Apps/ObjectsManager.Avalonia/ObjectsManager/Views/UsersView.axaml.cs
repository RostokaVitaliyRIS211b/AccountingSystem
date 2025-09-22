using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ObjectsManager.Interfaces;
using System;

namespace ObjectsManager.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    private void SaveButtClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is IUsersViewModel { SelectedUser : not null } viewModel)
        {
            viewModel.SelectedUser.Password = saveButt.Text ?? "";
        }
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
    {
        if (DataContext is IUsersViewModel viewModel) 
        {
            viewModel.UpdateCheckBox();
        }
    }
}