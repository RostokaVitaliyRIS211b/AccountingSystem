using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Styling;

using CommunityToolkit.Mvvm.Input;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using MsBox.Avalonia;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;
using ObjectsManager.Windows;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            FilteredConObjects.Add(obj);
        }

        foreach(var producer in await Service.GetAllProducersAsync())
        {
            Producers.Add(producer);
        }

        foreach(var name in await Service.GetAllNamesAsync())
        {
            Names.Add(name);
        }

        foreach(var unitType in await Service.GetAllTypesOfUnitAsync())
        {
            TypesOfUnits.Add(unitType);
        }

        foreach(var type in await Service.GetAllTypesOfItemsAsync())
        {
            TypesOfItems.Add(type);
        }

        foreach(var prop in await Service.GetAllGroupingPropsAsync())
        {
            GroupingProperties.Add(prop);
        }
    }

    #endregion

    #region Properties

    private MainService Service { get; set; }

    public string SelectedObjName { get => SelectedObj?.Name ?? ""; set { } }

    private ConObject? _cobj;
    public ConObject? SelectedObj { get => _cobj; set { _cobj = value; OnPropertyChanged(nameof(SelectedObj)); OnPropertyChanged(nameof(SelectedObjName)); ConObjChanged(); } }


    private Item? _item;
    public Item? SelectedObjItem { get => _item; set { _item = value; OnPropertyChanged(nameof(SelectedObjItem)); } }


    public ObservableCollection<ConObject> ConObjects { get; } = [];

    public ObservableCollection<ConObject> FilteredConObjects { get; } = [];

    private string _filter = "";
    public string FilterObj { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterObj)); Filter(); } }

    public ThemeVariant Light => ThemeVariant.Light;

    public ThemeVariant Dark => ThemeVariant.Dark;

    public Window? Win { get; set; }


    private decimal? _groups = 0;
    public decimal? NumberOfGroups { get => _groups; set { _groups = value; OnPropertyChanged(nameof(NumberOfGroups)); } }


    public ObservableCollection<ItemWrapper> ItemsOfConObj { get; set; } = [];

    private ObservableCollection<NameItem> Names { get; } = [];

    private ObservableCollection<Producer> Producers { get; } = [];

    private ObservableCollection<TypeOfUnit> TypesOfUnits { get; } = [];

    private ObservableCollection<TypeOfItem> TypesOfItems { get; } = [];

    private ObservableCollection<GroupingProperty> GroupingProperties { get; } = [];

    #endregion

    #region Commands

    [RelayCommand]
    public async Task OpenRoleWindow()
    {
        try
        {
            var rolesWindow = new RolesWindow(new RolesViewModel(Service));
            
            await rolesWindow.ShowDialog(Win!);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна Роли -> {e.Message}")).ShowAsync();
        }

    }
    [RelayCommand]
    public async Task OpenUserWindow()
    {
        try
        {
            var userWindow = new UsersWindow(new UsersViewModel(Service));
            await userWindow.ShowDialog(Win!);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна Пользователи -> {e.Message}")).ShowAsync();
        }

    }

    [RelayCommand]
    public async Task OpenObjectsWindow()
    {

        try
        {
            var window = new ObjectsWindow(new ObjectsViewModel(Service));
            if (Win != null)
            {
                await window.ShowDialog(Win);
                RefreshObjects();
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна объекты -> {e.Message}")).ShowAsync();
        }
    }

    [RelayCommand]
    public async Task AddItem()
    {

        try
        {

        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении записи -> {e.Message}")).ShowAsync();
        }
    }

    #endregion

    private void Filter()
    {
        FilteredConObjects.Clear();
        foreach (var obj in ConObjects.Where(x => x.Name.Contains(FilterObj, StringComparison.OrdinalIgnoreCase)))
        {
            FilteredConObjects.Add(obj);
        }
    }

    private void RefreshObjects()
    {
        ConObjects.Clear();
        FilterObj = "";
        SelectedObj = null;
        SelectedObjItem = null;

        try
        {
            foreach (var obj in Service.GetAllObjects())
            {
                ConObjects.Add(obj);
            }
            FilterObj = "";
        }
        catch 
        {

        }

    }

    private void ConObjChanged()
    {
        try
        {
            ItemsOfConObj.Clear();
            foreach (var item in Service.GetItemsByObject(SelectedObj?.Id ?? -1))
            {
                var props = Service.GetGroupingPropsByItem(item.Id);
                var wrapper = new ItemWrapper(item, [.. props]);
                ItemsOfConObj.Add(wrapper);
            }
        }
        catch
        {

        }
    }

   
}
