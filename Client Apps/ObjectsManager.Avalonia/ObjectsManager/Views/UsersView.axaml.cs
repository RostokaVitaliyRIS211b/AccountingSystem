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
    private bool isChangingUser = true;
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is IUsersViewModel viewModel && !isChangingUser)
        {
            viewModel.SaveChanges();
        }
    }
    private void SaveButtClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is IUsersViewModel { SelectedUser : not null } viewModel)
        {
            viewModel.SelectedUser.Password = saveButt.Text ?? "";
            viewModel.SaveChanges();
        }
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) 
    {
        isChangingUser = true;
        if (DataContext is IUsersViewModel viewModel) 
        {
            viewModel.UpdateCheckBox();
        }
        isChangingUser = false;
    }
}