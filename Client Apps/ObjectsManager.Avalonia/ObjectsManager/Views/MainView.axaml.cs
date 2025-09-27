using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;

using ObjectsManager.Interfaces;
using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectsManager.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    IDisposable? _selectFileInteractionDisposable;
    IDisposable? _saveFileInteractionDisposable;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _selectFileInteractionDisposable?.Dispose();
        _saveFileInteractionDisposable?.Dispose();

        if (DataContext is IMainViewModel viewModel)
        {
            viewModel.GetSelectedItems = GetSelected;
            viewModel.FilterGrid = Filter;
            _selectFileInteractionDisposable = viewModel.LoadItems.RegisterHandler(InteractionHandler);
            _saveFileInteractionDisposable = viewModel.SaveItems.RegisterHandler(SaveInteractionHandler);
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


    private static string? LoadFilePath = AppDomain.CurrentDomain.BaseDirectory;
    private async Task<string?> InteractionHandler(object? input)
    {
        // Get a reference to our TopLevel (in our case the parent Window)
        var topLevel = TopLevel.GetTopLevel(this);

        var folder = topLevel is not null ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(new Uri(LoadFilePath ?? AppDomain.CurrentDomain.BaseDirectory)) : null;

        // Try to get the files
        var storageFile = await topLevel!.StorageProvider.OpenFilePickerAsync(
                        new FilePickerOpenOptions()
                        {
                            AllowMultiple = false,
                            Title = "Загрузить файл",
                            SuggestedStartLocation = folder,
                            FileTypeFilter =
                            [
                                new FilePickerFileType("Excel документ")
                                {
                                    Patterns=["*.xlsx"]
                                },
                            ]
                        });

        // Transform the files as needed and return them. If no file was selected, null will be returned
        LoadFilePath = storageFile?.Select(x => x.TryGetLocalPath() ?? "")?.Where(p => !string.IsNullOrWhiteSpace(p))?.FirstOrDefault();
        return LoadFilePath;
    }

    private static string? SaveFilePath = AppDomain.CurrentDomain.BaseDirectory;
    private async Task<string?> SaveInteractionHandler(object? negro)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var folder = topLevel is not null ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(new Uri(SaveFilePath ?? AppDomain.CurrentDomain.BaseDirectory)) : null;

        var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Сохранение записей",
            SuggestedFileName = "Items",
            DefaultExtension = $"xlsx",
            SuggestedStartLocation= folder,
            FileTypeChoices =
            [
                new FilePickerFileType($"Excel документ")
                {
                    Patterns=["*.xlsx"]
                }
            ]
        });
        SaveFilePath = file?.TryGetLocalPath();
        return SaveFilePath;
    }
}
