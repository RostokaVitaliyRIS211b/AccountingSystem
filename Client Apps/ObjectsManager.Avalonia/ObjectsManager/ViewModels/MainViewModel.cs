using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using MsBox.Avalonia;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;
using ObjectsManager.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels;

public partial class MainViewModel : ViewModelBase, IMainViewModel
{
    [Obsolete]
    public MainViewModel()
    {
        throw new NotImplementedException();
    }

    public MainViewModel(MainService service)
    {
        Service = service;
        _ = Init();
    }

    #region InitModel

    private async Task Init()
    {
        ConObjects.Clear();
        foreach (var obj in await Service.GetAllObjectsAsync())
        {
            ConObjects.Add(obj);
        }
    }

    #endregion

    #region Properties

    private MainService Service { get; set; }

    public string SelectedObjName { get => SelectedObj?.Name ?? ""; set { } }

    private ConObject? _cobj;
    public ConObject? SelectedObj { get => _cobj; set { _cobj = value; OnPropertyChanged(nameof(SelectedObj)); OnPropertyChanged(nameof(SelectedObjName)); } }


    private Item? _item;
    public Item? SelectedObjItem { get => _item; set { _item = value; OnPropertyChanged(nameof(SelectedObjItem)); } }


    public ObservableCollection<ConObject> ConObjects { get; } = [];


    private string _filter = "";
    public string FilterObj { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterObj)); } }


    #endregion

    #region Commands

    [RelayCommand]
    public async Task OpenRoleWindow()
    {
        try
        {
            var rolesWindow = new RolesWindow(new RolesViewModel(Service));
            rolesWindow.Show();
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна Роли -> {e.Message}")).ShowAsync();
        }
    }

    #endregion
}
