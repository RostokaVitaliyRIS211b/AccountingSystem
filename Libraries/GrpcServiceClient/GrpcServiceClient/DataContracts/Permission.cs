using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class Permission : INotifyPropertyChanged
    {
        public Permission()
        {
            ProtoObject = new ProtoPermission();
            Id = -1;
        }

        internal Permission(ProtoPermission proto)
        {
            ProtoObject = proto ?? new ProtoPermission();
        }

        #region ProtoFields

        internal ProtoPermission ProtoObject { get; init; }

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
