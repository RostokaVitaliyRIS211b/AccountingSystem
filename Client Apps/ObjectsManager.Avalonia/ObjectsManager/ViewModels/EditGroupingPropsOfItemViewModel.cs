using Avalonia.Controls;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using MsBox.Avalonia;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{


    public partial class EditGroupingPropsOfItemViewModel : ViewModelBase, IEditGroupingPropsOfItemViewModel
    {
        [Obsolete]
        public EditGroupingPropsOfItemViewModel()
        {
            throw new NotImplementedException();
        }

        public EditGroupingPropsOfItemViewModel(ItemWrapper wrapper, MainService service, ObservableCollection<GroupingProperty> properties)
        {
            GProps = properties;
            Service = service;
            Wrapper = wrapper;

            Init();
        }

        private ItemWrapper Wrapper { get; set; }

        private MainService Service { get; set; }

        private ObservableCollection<GroupingProperty> GProps { get; }

        public ObservableCollection<CheckGroupProp> ChGProps { get; set; } = [];

        public ObservableCollection<CheckGroupProp> ChGPropsAll { get; set; } = [];

        public Window? Win { get; set; }


        private string? _filter = "";
        public string? FilterGProp { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterGProp)); Filter(); } }


        private string? _newName;
        public string? NameOfNewProp { get => _newName; set { _newName = value; OnPropertyChanged(nameof(NameOfNewProp)); } }

        public ObservableCollection<GroupingProperty> SelectedProps { get; set; } = [];

        private void Init()
        {
            var propsOfItem = Wrapper.SourceItem.Id != -1 ? Service.GetGroupingPropsByItem(Wrapper.SourceItem.Id) : [];
            foreach (GroupingProperty prop in GProps)
            {
                var gay = new CheckGroupProp() { Property = prop, IsSelected = propsOfItem.Any(x => x.Id == prop.Id), SelectedChanged=IsSelectedChanged };
                ChGPropsAll.Add(gay);
            }

            if (propsOfItem.Count > 0)
            {
                int counter = 0;
                foreach (var item in propsOfItem)
                {
                    var chItem = ChGPropsAll.FirstOrDefault(x => x.Property.Id == item.Id);
                    if (chItem != null)
                    {
                        var index = ChGPropsAll.IndexOf(chItem);
                        ChGPropsAll.Move(index, counter);
                        ++counter;
                    }
                }
            }

            Filter();
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task AddGroupingProp()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(NameOfNewProp))
                {
                    var gProp = new GroupingProperty { Name = NameOfNewProp };
                    gProp.Id = await Service.AddGroupingPropertyAsync(gProp);
                    if (gProp.Id == -1)
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении нового свойства группировки")).ShowAsync();
                        return;
                    }
                    GProps.Add(gProp);
                    ChGPropsAll.Add(new() { Property = gProp, IsSelected = false, SelectedChanged=IsSelectedChanged });
                    Filter();
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при добавлении нового свойства группировки -> {e.Message}")).ShowAsync();
            }
        }


        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task ApplyProps()
        {

            try
            {
                var props = ChGProps.Where(x => x.IsSelected).Select(x => x.Property).ToList();
                if (Wrapper.SourceItem.Id != -1)
                {

                    if (!await Service.SetGroupingPropertiesOfItemAsync(props, Wrapper.SourceItem.Id))
                    {
                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при изменении свойств группировки записи")).ShowAsync();
                        return;
                    }
                    else
                    {
                        Wrapper.GroupingProperties.Clear();
                        foreach (var prop in props)
                        {
                            Wrapper.GroupingProperties.Add(prop);
                        }

                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Свойства группировки записи успешно изменены")).ShowAsync();
                    }
                }
                else
                {
                    SelectedProps.Clear();
                    foreach (var prop in props)
                    {
                        SelectedProps.Add(prop);
                    }
                }

                Win?.Close();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при изменении свойств группировки записи -> {e.Message}")).ShowAsync();
            }

        }

        private void Filter()
        {
            ChGProps.Clear();
            foreach (var prop in ChGPropsAll.Where(x => x.Property.Name.Contains(FilterGProp ?? "", StringComparison.OrdinalIgnoreCase)))
            {
                ChGProps.Add(prop);
            }
        }

        private void IsSelectedChanged(CheckGroupProp prop)
        {
            var myIndex = ChGPropsAll.IndexOf(prop);
            var unS = ChGPropsAll.FirstOrDefault(x => !x.IsSelected && x.Property.Id != prop.Property.Id);
            var indexOfUnSelected = -1;

            if (!ChGPropsAll.Any(x => !x.IsSelected))
            {
                return;
            }


            if (unS is not null)
            {
                indexOfUnSelected = ChGPropsAll.IndexOf(unS);
            }
            else
            {
                return;
            }

            if (prop.IsSelected)
            {
                if (myIndex > indexOfUnSelected)
                {
                    ChGPropsAll.Move(myIndex, indexOfUnSelected);
                }
            }
            else
            {
                if(indexOfUnSelected - myIndex > 1)
                {
                    ChGPropsAll.Move(myIndex, indexOfUnSelected);
                }
            }
            Filter();
        }
    }

    public class CheckGroupProp : INotifyPropertyChanged
    {
        public required GroupingProperty Property { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;


        private bool _isSelected = false;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); SelectedChanged?.Invoke(this); } }

        public Action<CheckGroupProp>? SelectedChanged { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
