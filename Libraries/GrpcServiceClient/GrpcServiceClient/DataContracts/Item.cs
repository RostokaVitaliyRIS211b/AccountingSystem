using AccountingSystemService;

using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class Item : INotifyPropertyChanged
    {
        public Item()
        {
            ProtoObject = new ProtoItem();
            ProtoObject.Obj = new();
            ProtoObject.UnitType = new();
            ProtoObject.Producer = new();
            ProtoObject.Type = new();
            ProtoObject.NameItem = new();
        }

        internal Item(ProtoItem proto)
        {
            ProtoObject = proto ?? new ProtoItem();

            if (ProtoObject.Obj != null)
                obj = new ConObject(ProtoObject.Obj);
            else
            {
                ProtoObject.Obj = new();
            }

            if (ProtoObject.UnitType != null)
                unitType = new TypeOfUnit(ProtoObject.UnitType);
            else
            {
                ProtoObject.UnitType = new();
            }

            if (ProtoObject.Producer != null)
                producer = new Producer(ProtoObject.Producer);
            else
            {
                ProtoObject.Producer = new();
            }

            if (ProtoObject.Type != null)
                type = new TypeOfItem(ProtoObject.Type);
            else
            {
                ProtoObject.Type = new();
            }

            if (ProtoObject.NameItem != null)
                nameItem = new NameItem(ProtoObject.NameItem);
            else
            {
                ProtoObject.NameItem = new();
            }
        }

        #region ProtoFields

        internal ProtoItem ProtoObject { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => ProtoObject.Id;
            set
            {
                ProtoObject.Id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        private ConObject? obj;
        public ConObject? Obj
        {
            get => obj;
            set
            {
                obj = value;
                ProtoObject.Obj = value?.ProtoObject;
                RaisePropertyChanged(nameof(Obj));
            }
        }

        private TypeOfUnit? unitType;
        public TypeOfUnit? UnitType
        {
            get => unitType;
            set
            {
                unitType = value;
                ProtoObject.UnitType = value?.ProtoObject;
                RaisePropertyChanged(nameof(UnitType));
            }
        }

        public double CountOfUnits
        {
            get => ProtoObject.CountOfUnits;
            set
            {
                ProtoObject.CountOfUnits = value;
                RaisePropertyChanged(nameof(CountOfUnits));
            }
        }

        public double PricePerUnit
        {
            get => ProtoObject.PricePerUnit;
            set
            {
                ProtoObject.PricePerUnit = value;
                RaisePropertyChanged(nameof(PricePerUnit));
            }
        }

        public double ExpectedCost
        {
            get => ProtoObject.ExpectedCost;
            set
            {
                ProtoObject.ExpectedCost = value;
                RaisePropertyChanged(nameof(ExpectedCost));
            }
        }

        private Producer? producer;
        public Producer? Producer
        {
            get => producer;
            set
            {
                producer = value;
                ProtoObject.Producer = value?.ProtoObject;
                RaisePropertyChanged(nameof(Producer));
            }
        }

        public string Description
        {
            get => ProtoObject.Description;
            set
            {
                ProtoObject.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        private TypeOfItem? type;
        public TypeOfItem? Type
        {
            get => type;
            set
            {
                type = value;
                ProtoObject.Type = value?.ProtoObject;
                RaisePropertyChanged(nameof(Type));
            }
        }

        private NameItem? nameItem;
        public NameItem? NameItem
        {
            get => nameItem;
            set
            {
                nameItem = value;
                ProtoObject.NameItem = value?.ProtoObject;
                RaisePropertyChanged(nameof(NameItem));
            }
        }

        public double CountOfUsedUnits
        {
            get => ProtoObject.CountOfUsedUnits;
            set
            {
                ProtoObject.CountOfUsedUnits = value;
                RaisePropertyChanged(nameof(CountOfUsedUnits));
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
