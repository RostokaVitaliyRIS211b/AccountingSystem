using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

using ObjectsManager.Interfaces;

using System;
using System.Threading.Tasks;

namespace ObjectsManager.Views;

public partial class ProgressBarView : UserControl
{
    public ProgressBarView()
    {
        InitializeComponent();
    }

    IDisposable? _saveFileInteractionDisposable;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _saveFileInteractionDisposable?.Dispose();

        if (DataContext is IProgressBarViewModel viewModel)
        {
            _saveFileInteractionDisposable = viewModel.Save.RegisterHandler(SaveInteractionHandler);
        }
    }

    private static string? SaveFilePath = AppDomain.CurrentDomain.BaseDirectory;
    private async Task<string?> SaveInteractionHandler(object? negro)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var folder = topLevel is not null ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(new Uri(SaveFilePath ?? AppDomain.CurrentDomain.BaseDirectory)) : null;

        var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = $"Сохранение записей",
            SuggestedFileName = $"Backup {DateTime.Now}",
            DefaultExtension = $"backup",
            SuggestedStartLocation = folder,
            FileTypeChoices =
            [
                new FilePickerFileType($"Бэкап базы данных")
                {
                    Patterns=["*.backup"]
                }
            ]
        });
        SaveFilePath = file?.TryGetLocalPath();
        return SaveFilePath;
    }

    private async void Button_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        if(DataContext is IProgressBarViewModel viewModel)
        {
            await viewModel.StopBackup();
        }
    }
}