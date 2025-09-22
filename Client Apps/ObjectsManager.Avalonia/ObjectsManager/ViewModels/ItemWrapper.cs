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
    public class ItemWrapper:INotifyPropertyChanged,IDisposable
    {
        public ItemWrapper(NameItem name, Producer producer, TypeOfItem type, TypeOfUnit unit, ConObject? obj, 
            double expectedCost, double countOfUnits, double pricePerUnit, double countOfUsedUnits,string description,
            params GroupingProperty[] groupingProperties)
        {
            SourceItem = new();
            SourceItem.NameItem = name;
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
            SourceItem.PropertyChanged += SourceItem_PropertyChanged;
        }

        public ItemWrapper(Item item, params GroupingProperty[] groupingProperties)
        {
            SourceItem = item;
            foreach (var groupingProperty in groupingProperties)
            {
                GroupingProperties.Add(groupingProperty);
            }
            SourceItem.PropertyChanged += SourceItem_PropertyChanged;
        }

        private void SourceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Item.CountOfUnits) || e.PropertyName == nameof(Item.CountOfUsedUnits)
                || e.PropertyName == nameof(Item.ExpectedCost) || e.PropertyName == nameof(Item.PricePerUnit) )
            {
                OnPropertyChanged(nameof(RemainsUnits));
                OnPropertyChanged(nameof(Overspend));
                OnPropertyChanged(nameof(RealSpend));
            }
        }

        public Item SourceItem { get; }

        public double RemainsUnits => SourceItem.CountOfUnits - SourceItem.CountOfUsedUnits;

        public double Overspend => RealSpend - SourceItem.ExpectedCost;

        public double RealSpend => SourceItem.CountOfUnits * SourceItem.PricePerUnit;

        public ObservableCollection<GroupingProperty> GroupingProperties { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            SourceItem.PropertyChanged -= SourceItem_PropertyChanged;
        }
    }
}
