using BdClasses;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class RoleWrapper : INotifyPropertyChanged
    {
        public RoleWrapper()
        {
            
            ProtoObject = new ProtoRole();
            InitializeCollections();
            Id = -1;
        }

        internal RoleWrapper(ProtoRole proto)
        {
            ProtoObject = proto ?? new ProtoRole();
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            permissions = new ObservableCollection<int>(
                ProtoObject.Permissions);
            permissions.CollectionChanged += OnPermissionsCollectionChanged;
        }

        #region ProtoFields

        internal ProtoRole ProtoObject { get; init; }

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

        private ObservableCollection<int> permissions;
        public ObservableCollection<int> Permissions
        {
            get => permissions;
            set
            {
                if (permissions != null)
                    permissions.CollectionChanged -= OnPermissionsCollectionChanged;

                permissions = value ?? new ObservableCollection<int>();
                permissions.CollectionChanged += OnPermissionsCollectionChanged;

                SyncPermissions();
                RaisePropertyChanged(nameof(Permissions));
            }
        }

        private void OnPermissionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncPermissions();
            RaisePropertyChanged(nameof(Permissions));
        }

        private void SyncPermissions()
        {
            ProtoObject.Permissions.Clear();
            ProtoObject.Permissions.AddRange(permissions);
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
