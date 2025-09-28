using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ObjectsManager.Interfaces;

namespace ObjectsManager.Windows;

public partial class ProgressBarWindow : Window
{
    public ProgressBarWindow(IProgressBarViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

       

        if(DataContext is IProgressBarViewModel viewModel && viewModel.IsBackupInProgress)
        {
            e.Cancel = true;
            await viewModel.StopBackup();
        }
    }
}