using Avalonia.Controls;

using ObjectsManager.Interfaces;

using System;

namespace ObjectsManager.Windows;

public partial class MainWindow : Window
{
    public MainWindow(IMainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.Win = this;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if(DataContext is IMainViewModel viewModel)
        {
            viewModel.Dispose();
        }
    }
}
