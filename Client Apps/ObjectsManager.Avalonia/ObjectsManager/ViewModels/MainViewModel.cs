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
using ObjectsManager.Core;

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

        foreach (var producer in await Service.GetAllProducersAsync())
        {
            Producers.Add(producer);
        }

        foreach (var name in await Service.GetAllNamesAsync())
        {
            Names.Add(name);
        }

        foreach (var unitType in await Service.GetAllTypesOfUnitAsync())
        {
            TypesOfUnits.Add(unitType);
        }

        foreach (var type in await Service.GetAllTypesOfItemsAsync())
        {
            TypesOfItems.Add(type);
        }

        foreach (var prop in await Service.GetAllGroupingPropsAsync())
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


    private ItemWrapper? _item;
    public ItemWrapper? SelectedObjItem { get => _item; set { _item = value; OnPropertyChanged(nameof(SelectedObjItem)); } }


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

    public ObservableCollection<ItemWrapper> SelectedItemsOfConObj { get; set; } = [];

    private ObservableCollection<NameItem> Names { get; } = [];

    private ObservableCollection<Producer> Producers { get; } = [];

    private ObservableCollection<TypeOfUnit> TypesOfUnits { get; } = [];

    private ObservableCollection<TypeOfItem> TypesOfItems { get; } = [];

    private ObservableCollection<GroupingProperty> GroupingProperties { get; } = [];


    private string? itemFilter = "";
    public string? FilterItem { get => itemFilter; set { itemFilter = value; OnPropertyChanged(nameof(FilterItem)); FilterGrid?.Invoke(); } }

    public Func<IEnumerable<ItemWrapper>>? GetSelectedItems { get; set; }

    public Action? FilterGrid { get; set; }

    public Interaction<object?, string?> LoadItems { get; } = new();

    public Interaction<object?, string?> SaveItems { get; } = new();

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
            var dataContext = new AddItemViewModel(null, SelectedObj, Service, GroupingProperties);
            var window = new AddItemWindow(dataContext);
            window.Title += SelectedObjName;
            if (Win != null)
            {
                await window.ShowDialog(Win);
                if (dataContext.Wrapper.SourceItem.Id != -1)
                {
                    ItemsOfConObj.Add(dataContext.Wrapper);
                }
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении записи -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task EditItem()
    {
        try
        {
            if (SelectedObjItem is null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите запись для редактирования")).ShowAsync();
                return;
            }

            var dataContext = new AddItemViewModel(SelectedObjItem, SelectedObj, Service, GroupingProperties);
            var window = new AddItemWindow(dataContext);
            window.Title = $"Редактикрование свойств записи {SelectedObjItem.SourceItem.NameItem?.Name}";

            if (Win != null)
            {
                await window.ShowDialog(Win);
            }
        }
        catch (Exception e)
        {

            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна редактирования записи -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task EditGroupingPropertiesOfItem()
    {
        try
        {
            if (SelectedObjItem is null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите запись для редактирования свойств группировки")).ShowAsync();
                return;
            }

            var dataContext = new EditGroupingPropsOfItemViewModel(SelectedObjItem, Service, GroupingProperties);
            var window = new EditGroupingPropsOfItemWindow(dataContext);
            window.Title += SelectedObjItem.SourceItem.NameItem?.Name ?? "";

            if (Win != null)
            {
                await window.ShowDialog(Win);
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна редактирования свойств группировки -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task DeleteItem()
    {
        if (SelectedObjItem is null)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите запись для редактирования свойств группировки")).ShowAsync();
            return;
        }

        try
        {
            var res = await MessageBoxManager.GetMessageBoxCustom(MessageBoxParamsHelper.GetYesOrNoQuestionBoxParams("Вопрос", $"Удалить запись {SelectedObjItem.SourceItem.NameItem?.Name} ?")).ShowAsync();
            if (res != "Да")
            {
                return;
            }

            if (await Service.RemoveItemAsync(SelectedObjItem.SourceItem))
            {
                ItemsOfConObj.Remove(SelectedObjItem);
                SelectedObjItem = null;
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении записи")).ShowAsync();
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении записи -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task AddGroupingPropToSelectedItems()
    {
        try
        {
            var dataContext = new EditGroupingPropsOfItemViewModel(new ItemWrapper(new Item() { Id = -1 }), Service, GroupingProperties);
            var window = new EditGroupingPropsOfItemWindow(dataContext);
            window.Title = "Выбор свойств группировки для добавления";

            if (Win is not null)
            {
                await window.ShowDialog(Win);
            }

            if (dataContext.SelectedProps.Count > 0)
            {
                var selected = GetSelectedItems?.Invoke();
                if (selected != null)
                {
                    foreach (var item in selected)
                    {
                        foreach (var prop in dataContext.SelectedProps)
                        {
                            if (item.GroupingProperties.FirstOrDefault(x => x.Id == prop.Id) is null)
                            {
                                if (await Service.AddGroupingPropertyOfItemAsync(prop, item.SourceItem.Id))
                                {
                                    item.GroupingProperties.Add(prop);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна для добавления свойства группировки -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task DeleteGroupingPropOfSelectedItems()
    {
        try
        {
            var dataContext = new EditGroupingPropsOfItemViewModel(new ItemWrapper(new Item() { Id = -1 }), Service, GroupingProperties);
            var window = new EditGroupingPropsOfItemWindow(dataContext);
            window.Title = "Выбор свойств группировки для удаления";

            if (Win is not null)
            {
                await window.ShowDialog(Win);
            }

            if (dataContext.SelectedProps.Count > 0)
            {
                var selected = GetSelectedItems?.Invoke();
                if (selected != null)
                {
                    foreach (var item in selected)
                    {
                        foreach (var prop in dataContext.SelectedProps)
                        {
                            if (item.GroupingProperties.FirstOrDefault(x => x.Id == prop.Id) is GroupingProperty pr)
                            {
                                if (await Service.RemoveGroupingPropertyOfItemAsync(prop, item.SourceItem.Id))
                                {
                                    item.GroupingProperties.Remove(pr);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна для удаления свойства группировки -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task SetGroupingPropsOfSelectedItems()
    {
        try
        {
            var dataContext = new EditGroupingPropsOfItemViewModel(new ItemWrapper(new Item() { Id = -1 }), Service, GroupingProperties);
            var window = new EditGroupingPropsOfItemWindow(dataContext);
            window.Title = "Выбор свойств группировки";

            if (Win is not null)
            {
                await window.ShowDialog(Win);
            }

            if (dataContext.SelectedProps.Count > 0)
            {
                var selected = GetSelectedItems?.Invoke();
                if (selected != null)
                {
                    foreach (var item in selected)
                    {
                        if (await Service.SetGroupingPropertiesOfItemAsync([.. dataContext.SelectedProps], item.SourceItem.Id))
                        {
                            item.GroupingProperties.Clear();
                            foreach (var prop in dataContext.SelectedProps)
                            {
                                item.GroupingProperties.Add(prop);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна для редактирования свойств группировки у выбранных элементов -> {e.Message}")).ShowAsync();
        }
    }


    [RelayCommand]
    public async Task OpenMetaDataWindow()
    {

        try
        {
            if (SelectedObjItem != null)
            {
                var dataContext = new MetaDataViewModel(SelectedObjItem, Service);
                var window = new MetaDataWindow(dataContext);
                window.Title += $"{SelectedObjItem.SourceItem.NameItem?.Name}";

                if (Win is not null)
                {
                    await window.ShowDialog(Win);
                }
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна просмотра метаданных -> {e.Message}")).ShowAsync();
        }

    }


    [RelayCommand]
    public async Task LoadItemsToObject()
    {

        try
        {
            if (SelectedObj is null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите объект!")).ShowAsync();
                return;
            }

            var path = await LoadItems.HandleAsync(null);

            var items = await ExcelSerializatorHelper.LoadExcelFile(path, Service, SelectedObj, TypesOfItems.ToList(), Names.ToList(), TypesOfUnits.ToList(), Producers.ToList()) ?? throw new Exception("Items not loaded");


            var selected = SelectedObj;
            SelectedObj = null;
            SelectedObj = selected;



            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Записи успешно загружены")).ShowAsync();
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при загрузке записей -> {e.Message}")).ShowAsync();
        }

    }


    [RelayCommand]
    public async Task SaveItemsOfObject()
    {
        try
        {
            if (SelectedObj is null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите объект!")).ShowAsync();
                return;
            }

            var path = await SaveItems.HandleAsync(null);

            var exception = ExcelSerializatorHelper.SaveAsExcelTable(path, SelectedObj, ItemsOfConObj.ToList());

            if (exception != null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при сохранении записей в файл -> {exception.Message}")).ShowAsync();
                return;
            }
            else
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Записи успешно сохранены")).ShowAsync();
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при сохранении записей -> {e.Message}")).ShowAsync();
        }

    }


    [RelayCommand]
    public async Task OpenObjectMetaDataWindow()
    {
        try
        {
            if (SelectedObj is null)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите объект!")).ShowAsync();
                return;
            }

            var dataContext = new ObjectMetaDataViewModel(SelectedObj, Service);
            var window = new MetaDataWindow(dataContext);

            window.Title = $"Дополнительная информация по объекту: {SelectedObj.Name}";

            if(Win is not null)
            {
                await window.ShowDialog(Win);
            }
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна Дополнительная информация по объекту -> {e.Message}")).ShowAsync();
        }
    }


    #endregion

    #region OtherMethods

    public bool FilteringItem(object item)
    {
        if (item is ItemWrapper wrapper)
        {
            return wrapper.SourceItem.NameItem?.Name.Contains(FilterItem ?? "", StringComparison.OrdinalIgnoreCase) ?? false;
        }
        return false;
    }

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

    public void Dispose()
    {
        Service.Dispose();
    }

    #endregion
}
