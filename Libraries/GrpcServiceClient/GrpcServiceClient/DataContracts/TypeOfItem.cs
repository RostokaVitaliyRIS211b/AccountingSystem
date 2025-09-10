using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class TypeOfItem : INotifyPropertyChanged
    {
        public TypeOfItem()
        {
            ProtoObject = new ProtoTypeOfItem();
        }

        internal TypeOfItem(ProtoTypeOfItem proto)
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
