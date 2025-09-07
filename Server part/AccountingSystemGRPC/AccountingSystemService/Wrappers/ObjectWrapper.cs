using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class ObjectWrapper : INotifyPropertyChanged
    {
        public ObjectWrapper()
        {
            ProtoObject = new ProtoObject();
        }

        internal ObjectWrapper(ProtoObject proto)
        {
            ProtoObject = proto ?? new ProtoObject();
        }

        #region ProtoFields

        internal ProtoObject ProtoObject { get; init; }

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

        public string Description
        {
            get => ProtoObject.Description;
            set
            {
                ProtoObject.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public string Address
        {
            get => ProtoObject.Address;
            set
            {
                ProtoObject.Address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
