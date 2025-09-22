using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

using ObjectsManager.Interfaces;
using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ObjectsManager.Views;

public partial class MainView : UserControl
{


    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if(DataContext is IMainViewModel viewModel)
        {
            viewModel.GetSelectedItems = GetSelected;
            viewModel.FilterGrid = Filter;
        }
    }

    private void Filter()
    {
        if (mainDataGrid.CollectionView is DataGridCollectionView view)
        {
            view.Refresh();
        }
    }

    private IEnumerable<ItemWrapper> GetSelected()
    {
        var result = new List<ItemWrapper>();
        foreach(ItemWrapper wrap in mainDataGrid.SelectedItems)
        {
            result.Add(wrap);
        }
        return result;
    }

    private void MenuItem_Theme_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(sender is MenuItem item && item.Tag is ThemeVariant variant && Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = variant;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is IMainViewModel viewModel && mainDataGrid.CollectionView is DataGridCollectionView view)
        {
            view.Filter = viewModel.FilteringItem;
        }
    }

    private void NumericUpDown_ValueChanged(object? sender, Avalonia.Controls.NumericUpDownValueChangedEventArgs e)
    {
        if(DataContext is IMainViewModel viewModel && mainDataGrid.CollectionView is DataGridCollectionView view &&
            sender is NumericUpDown numeric)
        {
            view.GroupDescriptions.Clear();

            try
            {
                var groups = (int)Math.Floor(numeric.Value ?? 0);
                if (groups > 0)
                {
                    for(int i = 1; i <= groups; i++)
                    {
                        view.GroupDescriptions.Add(new GroupingByProp(i));
                    }
                }
            }
            catch
            {

            }

            view.Refresh();
        }
    }

    private void DataGrid_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if(DataContext is IMainViewModel viewModel)
        {
            viewModel.SelectedItemsOfConObj.Clear();
            foreach(ItemWrapper wrapper in mainDataGrid.SelectedItems)
            {
                viewModel.SelectedItemsOfConObj.Add(wrapper);
            }
        }
    }
}
