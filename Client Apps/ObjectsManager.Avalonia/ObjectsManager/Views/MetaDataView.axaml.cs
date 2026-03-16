using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectsManager.Views;

public partial class MetaDataView : UserControl, IDisposable
{
    public MetaDataView()
    {
        InitializeComponent();
    }

    IDisposable? _selectFileInteractionDisposable;
    IDisposable? _saveFileInteractionDisposable;

    protected override void OnDataContextChanged(EventArgs e)
    {
        // Dispose any old handler
        _selectFileInteractionDisposable?.Dispose();
        _saveFileInteractionDisposable?.Dispose();

        if (DataContext is IMetaDataViewModel vm)
        {
            // register the interaction handler
            _selectFileInteractionDisposable = vm.LoadFileInter.RegisterHandler(InteractionHandler);
            _saveFileInteractionDisposable = vm.SaveFileInter.RegisterHandler(SaveInteractionHandler);
        }

        base.OnDataContextChanged(e);
    }

    private async Task<string?> InteractionHandler(object? input)
    {
        // Get a reference to our TopLevel (in our case the parent Window)
        var topLevel = TopLevel.GetTopLevel(this);

        // Try to get the files
        var storageFile = await topLevel!.StorageProvider.OpenFilePickerAsync(
                        new FilePickerOpenOptions()
                        {
                            AllowMultiple = false,
                            Title = "Загрузить файл",
                            FileTypeFilter =
                            [
                                new FilePickerFileType("Изображение")
                                {
                                    Patterns=["*.jpg","*.png"]
                                },
                                new FilePickerFileType("PDF Документ")
                                {
                                    Patterns=["*.pdf"]
                                }
                            ]
                        });

        // Transform the files as needed and return them. If no file was selected, null will be returned
        return storageFile?.Select(x => x.TryGetLocalPath() ?? "")?.Where(p => !string.IsNullOrWhiteSpace(p))?.FirstOrDefault();
    }

    private async Task<string?> SaveInteractionHandler(string input)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Сохранение файла типа {input}",
            SuggestedFileName = "Info",
            DefaultExtension = $"{input}",
            FileTypeChoices =
            [
                new FilePickerFileType($"Тип {input}")
                {
                    Patterns=[$"*.{input}"]
                }
            ]
        });
        return file?.TryGetLocalPath();
    }

    public CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is IMetaDataViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModelPropertyChange;
            _ = viewModel.SetTabItems(tabCtrl, TokenSource.Token);
        }
    }

    private void ViewModelPropertyChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IMetaDataViewModel.IsLoading) && DataContext is IMetaDataViewModel viewModel && !viewModel.IsLoading)
        {
            TokenSource.Cancel();
            TokenSource = new();
        }
        else if (e.PropertyName == nameof(IMetaDataViewModel.IsLoading) && DataContext is IMetaDataViewModel viewModel2 && viewModel2.IsLoading)
        {
            _ = StartLoadingAnimation(TokenSource.Token);
        }
    }

    private void Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is IMetaDataViewModel viewModel)
        {
            viewModel.Win?.Close();
        }
    }

    public async Task StartLoadingAnimation(CancellationToken token)
    {
        Dispatcher.UIThread.Invoke(() => { LoadingTextBlock.Text = "Загрузка"; });
        var delay = 300;
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(delay, token);
            Dispatcher.UIThread.Invoke(() => { LoadingTextBlock.Text = LoadingTextBlock.Text + "."; });
            await Task.Delay(delay, token);
            Dispatcher.UIThread.Invoke(() => { LoadingTextBlock.Text = LoadingTextBlock.Text + "."; });
            await Task.Delay(delay, token);
            Dispatcher.UIThread.Invoke(() => { LoadingTextBlock.Text = LoadingTextBlock.Text + "."; });
            await Task.Delay(delay, token);
            Dispatcher.UIThread.Invoke(() => { LoadingTextBlock.Text = "Загрузка"; });
        }
    }

    public void Dispose()
    {
        TokenSource.Cancel();
        TokenSource.Dispose();
    }

    private void Refresh_Cache_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is IMetaDataViewModel viewModel)
        {
            tabCtrl.Items.Clear();
            viewModel.ClearCache();
            _ = viewModel.SetTabItems(tabCtrl, TokenSource.Token);
        }
    }
}