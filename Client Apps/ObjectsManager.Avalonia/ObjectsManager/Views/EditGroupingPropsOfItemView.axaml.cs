using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

using ObjectsManager.Interfaces;
using ObjectsManager.ViewModels;

using System.Linq;

namespace ObjectsManager.Views;

public partial class EditGroupingPropsOfItemView : UserControl
{
    public EditGroupingPropsOfItemView()
    {
        InitializeComponent();
       
    }

    private ListBoxItem ghostItem;

    private void Button_Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(DataContext is IEditGroupingPropsOfItemViewModel viewModel)
        {
            viewModel.Win?.Close();
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
    }

    private void ListBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (mainList.SelectedItem is CheckGroupProp { IsSelected : true } prop && DataContext is IEditGroupingPropsOfItemViewModel viewModel)
        {
            var index = mainList.SelectedIndex;
            var newIndex = index;
            var colIndex = viewModel.ChGPropsAll.IndexOf(prop);
            var newColIndex = colIndex;
            var indexOfUns = viewModel.ChGProps.FirstOrDefault(x => !x.IsSelected) is CheckGroupProp prop2 ?
                viewModel.ChGProps.IndexOf(prop2) : -1;

            if (e.PhysicalKey == PhysicalKey.W && index > 0)
            {
                --newIndex;
                --colIndex;
            }
            else if(e.PhysicalKey == PhysicalKey.S && index < indexOfUns - 1)
            {
                ++newIndex;
                ++newColIndex;
            }
            viewModel.ChGPropsAll.Move(colIndex, newColIndex);
            viewModel.ChGProps.Move(index, newIndex);
            mainList.SelectedItem = prop;
            mainList.Focus();
        }
        e.Handled = true;
    }
}