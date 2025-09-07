using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class PermissionWrapper : INotifyPropertyChanged
    {
        public PermissionWrapper()
        {
            ProtoObject = new ProtoPermission();
            Id = -1;
        }

        internal PermissionWrapper(ProtoPermission proto)
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
