using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public class ItemWrapper:INotifyPropertyChanged
    {
        public ItemWrapper(NameItem name, Producer producer, TypeOfItem type, TypeOfUnit unit, ConObject obj, 
            int expectedCost, int countOfUnits, int pricePerUnit, int countOfUsedUnits,string description,
            params GroupingProperty[] groupingProperties)
        {
            SourceItem = new();
            SourceItem.Producer = producer;
            SourceItem.Type = type;
            SourceItem.UnitType = unit;
            SourceItem.Obj = obj;
            SourceItem.PricePerUnit = pricePerUnit;
            SourceItem.CountOfUnits = countOfUnits;
            SourceItem.CountOfUsedUnits = countOfUsedUnits;
            SourceItem.Description = description;
            SourceItem.Id = -1;
            foreach (var groupingProperty in groupingProperties)
            {
                GroupingProperties.Add(groupingProperty);
            }
        }

        public ItemWrapper(Item item, params GroupingProperty[] groupingProperties)
        {
            SourceItem = item;
            foreach (var groupingProperty in groupingProperties)
            {
                GroupingProperties.Add(groupingProperty);
            }
        }

        public Item SourceItem { get; }

        public int RemainsUnits => SourceItem.CountOfUnits - SourceItem.CountOfUsedUnits;

        public int Overspend =>  SourceItem.CountOfUnits * SourceItem.PricePerUnit - SourceItem.ExpectedCost;

        public ObservableCollection<GroupingProperty> GroupingProperties { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
