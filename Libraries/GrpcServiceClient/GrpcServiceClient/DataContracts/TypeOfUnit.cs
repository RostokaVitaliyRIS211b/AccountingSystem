using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class TypeOfUnit : INotifyPropertyChanged
    {
        public TypeOfUnit()
        {
            ProtoObject = new ProtoTypeOfUnit();
        }

        internal TypeOfUnit(ProtoTypeOfUnit proto)
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
