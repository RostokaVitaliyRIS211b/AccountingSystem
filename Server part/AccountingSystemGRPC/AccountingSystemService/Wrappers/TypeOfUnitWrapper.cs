using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class TypeOfUnitWrapper : INotifyPropertyChanged
    {
        public TypeOfUnitWrapper()
        {
            ProtoObject = new ProtoTypeOfUnit();
        }

        internal TypeOfUnitWrapper(ProtoTypeOfUnit proto)
        {
            ProtoObject = proto ?? new ProtoTypeOfUnit();
        }

        #region ProtoFields

        internal ProtoTypeOfUnit ProtoObject { get; init; }

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

        public string Name
        {
            get => ProtoObject.Name;
            set
            {
                ProtoObject.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
