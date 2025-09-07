using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class TypeOfItemWrapper : INotifyPropertyChanged
    {
        public TypeOfItemWrapper()
        {
            ProtoObject = new ProtoTypeOfItem();
        }

        internal TypeOfItemWrapper(ProtoTypeOfItem proto)
        {
            ProtoObject = proto ?? new ProtoTypeOfItem();
        }

        #region ProtoFields

        internal ProtoTypeOfItem ProtoObject { get; init; }

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
