using Avalonia.Controls;

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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class AddItemViewModel:ViewModelBase, IAddItemViewModel
    {
        [Obsolete]
        public AddItemViewModel()
        {
            throw new NotImplementedException();
        }

        public AddItemViewModel(ItemWrapper? wrapper, ConObject? conObject ,MainService service,  ObservableCollection<GroupingProperty> groupingProperties)
        {
            GroupingProperties = groupingProperties;
            Service = service;
            Wrapper = wrapper ?? new ItemWrapper(new NameItem() { Id = -1 },new Producer() { Id = -1 }, new TypeOfItem() { Id = -1 },
                new TypeOfUnit() { Id = -1 }, conObject ,0,0,0,0,"");
            Init();
        }

        public ItemWrapper Wrapper { get; }

        public Window? Win { get; set; }

        private MainService Service { get; set; }

        private ObservableCollection<NameItem> NameItems { get; set; } = [];

        public ObservableCollection<TypeOfItem> TypeOfItems { get; set; } = [];

        private ObservableCollection<TypeOfUnit> TypeOfUnits { get; set; } = [];

        private ObservableCollection<Producer> Producers { get; set; } = [];

        private ObservableCollection<GroupingProperty> GroupingProperties { get; }

        private ObservableCollection<GroupingProperty> SelectedProps { get; set; } = [];

        private void Init()
        {
            foreach (var item in Service.GetAllNames())
            {
                NameItems.Add(item);
            }

            foreach(var item in Service.GetAllTypesOfItems())
            {
                TypeOfItems.Add(item);
               
            }

            if (Wrapper.SourceItem.Type?.Id > 0)
            {
                var MyNegro = TypeOfItems.FirstOrDefault(x => x.Id == Wrapper.SourceItem.Type?.Id);
                if( MyNegro != null)
                {
                    TypeOfItems.Remove(MyNegro);
                    TypeOfItems.Add(Wrapper.SourceItem.Type);
                }
            }

            foreach (var item in Service.GetAllTypesOfUnit())
            {
                TypeOfUnits.Add(item);
            }

            foreach(var item in Service.GetAllProducers())
            {
                Producers.Add(item);
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task ApplyChanges()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Wrapper.SourceItem.NameItem?.Name))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Наименование пустое! Задайте наименование")).ShowAsync();
                    return;
                }

                if(NameItems.FirstOrDefault(x=>x.Name == Wrapper.SourceItem.NameItem?.Name) is NameItem item)
                {
                    Wrapper.SourceItem.NameItem = item;
                }
                else
                {
                    if(Wrapper.SourceItem.NameItem is not null)
                    {
                        Wrapper.SourceItem.NameItem.Id = await Service.AddNameItemAsync(Wrapper.SourceItem.NameItem);
                    }
                }

                if (string.IsNullOrWhiteSpace(Wrapper.SourceItem.UnitType?.Name))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Тип единицы измерения не задан! Задайте тип единицы измерения")).ShowAsync();
                    return;
                }

                if(TypeOfUnits.FirstOrDefault(x=>x.Name == Wrapper.SourceItem.UnitType?.Name) is TypeOfUnit unit)
                {
                    Wrapper.SourceItem.UnitType = unit;
                }
                else
                {
                    if(Wrapper.SourceItem.UnitType is not null)
                    {
                        Wrapper.SourceItem.UnitType.Id = await Service.AddTypeOfUnitAsync(Wrapper.SourceItem.UnitType);
                    }
                }

                if (string.IsNullOrWhiteSpace(Wrapper.SourceItem.Producer?.Name))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Производитель не задан! Задайте производителя")).ShowAsync();
                    return;
                }

                if(Producers.FirstOrDefault(x=>x.Name == Wrapper.SourceItem.Producer?.Name) is Producer producer)
                {
                    Wrapper.SourceItem.Producer ??= producer;
                    Wrapper.SourceItem.Producer.Id = producer.Id;
                    Wrapper.SourceItem.Producer.Name = producer.Name;
                }
                else
                {
                    if(Wrapper.SourceItem.Producer is not null)
                    {
                        Wrapper.SourceItem.Producer.Id = await Service.AddProducerAsync(Wrapper.SourceItem.Producer);
                    }
                }

                if(TypeOfItems.FirstOrDefault(x=>x.Id == Wrapper.SourceItem.Type?.Id) is null)
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Тип записи не задан! Задайте тип записи")).ShowAsync();
                    return;
                }



                if(Wrapper.SourceItem.Id == -1)
                {
                    Wrapper.SourceItem.Id = await Service.AddItemAsync(Wrapper.SourceItem);
                    if(Wrapper.SourceItem.Id == -1)
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении записи")).ShowAsync();
                        return;
                    }
                    else
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Новая запись успешно добавлена!")).ShowAsync();
                    }

                    if(SelectedProps.Count > 0)
                    {
                        if (await Service.SetGroupingPropertiesOfItemAsync([.. SelectedProps],Wrapper.SourceItem.Id))
                        {
                            Wrapper.GroupingProperties.Clear();
                            foreach (var prop in SelectedProps)
                            {
                                Wrapper.GroupingProperties.Add(prop);
                            }
                            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Свойства группировки для записи добавлены")).ShowAsync();
                        }
                        else
                        {
                            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении свойств группировки записи")).ShowAsync();
                            return;
                        }
                    }
                }
                else
                {
                    if(await Service.UpdateItemAsync(Wrapper.SourceItem))
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Запись успешно обновлена")).ShowAsync();
                    }
                    else
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при обновлении записи")).ShowAsync();
                        return;
                    }
                }

                Win?.Close();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при обновлении/добавлении записи -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task OpenNameSelectorWindow()
        {
            try
            {
                var dataContext = new NameSelectorViewModel(NameItems);
                var window = new NameSelectorWindow(dataContext);
                if(Win is not null)
                {
                    await window.ShowDialog(Win);
                }

                if(dataContext.SelectedName != null)
                {
                    Wrapper.SourceItem.NameItem ??= dataContext.SelectedName;
                    Wrapper.SourceItem.NameItem.Id = dataContext.SelectedName.Id;
                    Wrapper.SourceItem.NameItem.Name = dataContext.SelectedName.Name;
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна выбора наименования -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task OpenTypeOfUnitSelector()
        {
            try
            {
                var dataContext = new UnitTypeSelectorViewModel(TypeOfUnits);
                var window = new UnitTypeSelectorWindow(dataContext);
                if(Win is not null)
                {
                    await window.ShowDialog(Win);
                }

                if(dataContext.SelectedUnit is not null)
                {
                    Wrapper.SourceItem.UnitType ??= dataContext.SelectedUnit;
                    Wrapper.SourceItem.UnitType.Id = dataContext.SelectedUnit.Id;
                    Wrapper.SourceItem.UnitType.Name = dataContext.SelectedUnit.Name;
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна выбора типа единицы измерения -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task OpenProducerSelectorWindow()
        {
            try
            {
                var dataContext = new ProducerSelectorViewModel(Producers);
                var window = new ProducerSelectorWindow(dataContext);
                if (Win is not null)
                {
                    await window.ShowDialog(Win);
                }

                if(dataContext.SelectedProducer is not null)
                {
                    Wrapper.SourceItem.Producer ??= dataContext.SelectedProducer;
                    Wrapper.SourceItem.Producer.Id = dataContext.SelectedProducer.Id;
                    Wrapper.SourceItem.Producer.Name = dataContext.SelectedProducer.Name;
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна выбора производителя -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task EditGroupingProperties()
        {
            try
            {
                var dataContext = new EditGroupingPropsOfItemViewModel(Wrapper, Service, GroupingProperties);
                var window = new EditGroupingPropsOfItemWindow(dataContext);
                window.Title += Wrapper.SourceItem.NameItem?.Name ?? "";
                if (Win is not null)
                {
                    await window.ShowDialog(Win);
                }

                if(Wrapper.SourceItem.Id == -1)
                {
                    SelectedProps.Clear();
                    Wrapper.GroupingProperties.Clear();
                    foreach(var item in dataContext.SelectedProps)
                    {
                        SelectedProps.Add(item);
                        Wrapper.GroupingProperties.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна редактирования группировки  -> {e.Message}")).ShowAsync();
            }
        }

    }


}
