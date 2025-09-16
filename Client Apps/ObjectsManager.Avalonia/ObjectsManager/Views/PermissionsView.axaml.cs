using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ObjectsManager.Interfaces;
using ObjectsManager.ViewModels;

namespace ObjectsManager.Views;

public partial class PermissionsView : UserControl
{
    public PermissionsView()
    {
        InitializeComponent();
    }
    private void Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (Parent is Window window)
        {
            window.Close();
        }
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (grid.CollectionView is DataGridCollectionView view)
        {
            view.GroupDescriptions.Add(new Gay());
            view.Refresh();
        }
    }
    private void Button_Click_Add(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is IPermissionsViewModel viewModel) 
        {
            foreach (var item in viewModel.PermissionsCollection)
            {
                item.IsAllowed = true;
            }
        }
    }
    private void Button_Click_Del(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is IPermissionsViewModel viewModel)
        {
            foreach (var item in viewModel.PermissionsCollection)
            {
                item.IsAllowed = false;
            }
        }
    }
}