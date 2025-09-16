using CommunityToolkit.Mvvm.Input;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using MsBox.Avalonia;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class ObjectsViewModel:ViewModelBase,IObjectsViewModel
    {
        [Obsolete]
        public ObjectsViewModel()
        {
            throw new NotImplementedException();
        }

        public ObjectsViewModel(MainService service)
        {
            Service = service;
            ConObjects.Clear();
            foreach (var obj in Service.GetAllObjects())
            {
                ConObjects.Add(obj);
            }
            FilterObj = "";
        }

        private MainService Service { get; set; }

        public ObservableCollection<ConObject> ConObjects { get; } = [];

        public ObservableCollection<ConObject> FilteredConObjects { get; } = [];

        private string _filter = "";
        public string FilterObj { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterObj)); Filter(); } }

        public string SelectedObjName { get => SelectedObj?.Name ?? ""; set { } }

        private ConObject? _cobj;
        public ConObject? SelectedObj { get => _cobj; set { SelectedObjChanged(value); _cobj = value; OnPropertyChanged(nameof(SelectedObj)); OnPropertyChanged(nameof(SelectedObjName));  } }

        private void Filter()
        {
            FilteredConObjects.Clear();
            foreach (var obj in ConObjects.Where(x => x.Name.Contains(FilterObj, StringComparison.OrdinalIgnoreCase)))
            {
                FilteredConObjects.Add(obj);
            }
        }

        private void SelectedObjChanged(ConObject? obj)
        {
            if(SelectedObj != null)
            {
                SelectedObj.PropertyChanged -= Obj_PropertyChanged;
            }

            if(obj != null)
            {
                obj.PropertyChanged += Obj_PropertyChanged; ;
            }
        }

        private async void Obj_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(SelectedObj != null)
            {
                try
                {
                    if (!await Service.UpdateObjectAsync(SelectedObj))
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при обновлении объекта")).ShowAsync();
                    }
                }
                catch(Exception ex) 
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при обновлении объекта -> {ex.Message}")).ShowAsync();
                }
            }
        }

        [RelayCommand]
        public async Task AddObject()
        {

            try
            {
                var obj = new ConObject();
                obj.Name = $"Новый объект {DateTime.Now}";
                obj.Address = "";
                obj.Description = "";

                obj.Id = await Service.AddObjectAsync(obj);

                if(obj.Id == -1)
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении нового объекта")).ShowAsync();
                    return;
                }
                
                ConObjects.Add(obj);
                FilterObj = "";


                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Новый объект успешно добавлен")).ShowAsync();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении нового объекта -> {e.Message}")).ShowAsync();
            }
        }

        [RelayCommand]
        public async Task RemoveObject()
        {

            try
            {

                var res = await MessageBoxManager.GetMessageBoxCustom(MessageBoxParamsHelper.GetYesOrNoQuestionBoxParams("Вопрос", $"Удалить объект {SelectedObjName}?")).ShowAsync();
                if (res != "Да")
                {
                    return;
                }


                if(SelectedObj == null)
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Выберите объект для удаления")).ShowAsync();
                    return;
                }

                if(! await Service.RemoveObjectAsync(SelectedObj))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении объекта")).ShowAsync();
                    return;
                }

                ConObjects.Remove(SelectedObj);
                SelectedObj = null;
                Filter();


                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Объект успешно удален")).ShowAsync();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении объекта -> {e.Message}")).ShowAsync();
            }
        }
    }
}
