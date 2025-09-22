using BdClasses;

using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class ItemWrapper : INotifyPropertyChanged
    {
        public ItemWrapper()
        {
            ProtoObject = new ProtoItem();
            ProtoObject.Obj = new();
            ProtoObject.UnitType = new();
            ProtoObject.Producer = new();
            ProtoObject.Type = new();
            ProtoObject.NameItem = new();
        }

        internal ItemWrapper(ProtoItem proto)
        {
            ProtoObject = proto ?? new ProtoItem();

            if (ProtoObject.Obj != null)
                obj = new ObjectWrapper(ProtoObject.Obj);
            else
            {
                ProtoObject.Obj = new();
            }

            if (ProtoObject.UnitType != null)
                unitType = new TypeOfUnitWrapper(ProtoObject.UnitType);
            else
            {
                ProtoObject.UnitType = new();
            }

            if (ProtoObject.Producer != null)
                producer = new ProducerWrapper(ProtoObject.Producer);
            else
            {
                ProtoObject.Producer = new();
            }

            if (ProtoObject.Type != null)
                type = new TypeOfItemWrapper(ProtoObject.Type);
            else
            {
                ProtoObject.Type = new();
            }

            if (ProtoObject.NameItem != null)
                nameItem = new NameItemWrapper(ProtoObject.NameItem);
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

        private ObjectWrapper? obj;
        public ObjectWrapper? Obj
        {
            get => obj;
            set
            {
                obj = value;
                ProtoObject.Obj = value?.ProtoObject;
                RaisePropertyChanged(nameof(Obj));
            }
        }

        private TypeOfUnitWrapper? unitType;
        public TypeOfUnitWrapper? UnitType
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

        private ProducerWrapper? producer;
        public ProducerWrapper? Producer
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

        private TypeOfItemWrapper? type;
        public TypeOfItemWrapper? Type
        {
            get => type;
            set
            {
                type = value;
                ProtoObject.Type = value?.ProtoObject;
                RaisePropertyChanged(nameof(Type));
            }
        }

        private NameItemWrapper? nameItem;
        public NameItemWrapper? NameItem
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
