using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaComponents.MetaDataView.Helpers;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using MsBox.Avalonia;

using ObjectsManager.Core;
using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class ObjectMetaDataViewModel:ViewModelBase,IMetaDataViewModel
    {
        public ObjectMetaDataViewModel(ConObject obj, MainService service)
        {
            Obj = obj;
            Service = service;
        }

        private ConObject Obj { get; }

        private MainService Service { get; }

        private TabControl? MainTabControl { get; set; }

        public Window? Win { get; set; }

        public Interaction<object?, string?> LoadFileInter { get; } = new();

        public Interaction<string, string?> SaveFileInter { get; } = new();

        private List<ObjectMetadata> AddedMetaData { get; } = [];

        private List<ObjectMetadata> RemovedMetaData { get; } = [];

        private List<MetaDataType> AllMetaDataTypes { get; set; } = [];


        private TabItem? tSI;
        public TabItem? SelectedTabItem { get => tSI; set { tSI = value; OnPropertyChanged(nameof(SelectedTabItem)); } }


        public IEnumerable<TabItem> SetTabItems(TabControl tabControl)
        {
            var result = new List<TabItem>();
            this.MainTabControl = tabControl;
            try
            {
                var metaData = Service.GetAllObjectMetaData(Obj.Id);
                var types = Service.GetAllMetaDataTypes();
                AllMetaDataTypes = [.. types];
                foreach (var item in metaData)
                {
                    AddMetaDataToTabCtrl(item);
                }
            }
            catch(Exception e)
            {
                _ = MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при загрузке метаданных объекта -> {e.Message}")).ShowAsync();
                Win?.Close();
            }
           
            return result;
        }

        private void AddMetaDataToTabCtrl(ObjectMetadata item)
        {
            var type = AllMetaDataTypes.FirstOrDefault(x => x.Id == item.TypeId)?.Name;
            switch (type)
            {
                case "pdf":
                    MainTabControl?.Items.Add(ViewerHelper.CreatePdfTabItem(item.Name, item.Data, item, Closed));
                    break;
                case "png":
                case "jpg":
                    MainTabControl?.Items.Add(ViewerHelper.CreateImageTabItem(item.Name, item.Data, item, Closed));
                    break;
            }
        }


        private void Closed(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: TabItem { Tag: ObjectMetadata data } tabItem } btn)
            {
                if (MainTabControl?.Items.Remove(tabItem) ?? false)
                {
                    RemovedMetaData.Add(data);
                }
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task LoadFile()
        {
            try
            {
                var filePath = await LoadFileInter.HandleAsync(null);
                if (!string.IsNullOrEmpty(filePath))
                {
                    var ext = Path.GetExtension(filePath)[1..];
                    var fName = Path.GetFileName(filePath);
                    var type = AllMetaDataTypes.FirstOrDefault(t => t.Name == ext);
                    var data = File.ReadAllBytes(filePath);
                    if (type != null)
                    {
                        var itemMetaData = new ObjectMetadata();
                        itemMetaData.Name = fName;
                        itemMetaData.Data = data;
                        itemMetaData.ObjId = Obj.Id;
                        itemMetaData.TypeId = type.Id;
                        AddMetaDataToTabCtrl(itemMetaData);
                        AddedMetaData.Add(itemMetaData);
                    }
                    else
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Тип данных {ext} не поддерживается")).ShowAsync();
                    }
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при загрузке файла -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task SaveFile()
        {
            try
            {
                if (SelectedTabItem is not null && SelectedTabItem.Tag is ItemMetaData data)
                {
                    var ext = AllMetaDataTypes.FirstOrDefault(x => x.Id == data.TypeId);
                    if (ext != null)
                    {
                        var fileName = await SaveFileInter.HandleAsync(ext.Name);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            File.WriteAllBytes(fileName, data.Data);
                            await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Файл успешно сохранен!")).ShowAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при сохранении файла -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task ApplyChanges()
        {

            try
            {
                foreach (var item in RemovedMetaData)
                {
                    await Service.RemoveObjectMetadataAsync(item);
                }

                foreach (var item in AddedMetaData)
                {
                    await Service.AddObjectMetaDataAsync(item);
                }

                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Метаданные изменены")).ShowAsync();

                Win?.Close();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при изменении метаданных записи -> {e.Message}")).ShowAsync();
            }
        }


    }
}
