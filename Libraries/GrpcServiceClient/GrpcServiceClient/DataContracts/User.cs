using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class User : INotifyPropertyChanged
    {
        public User()
        {
            ProtoObject = new ProtoUser();
            InitializeCollections();
            Id = -1;
        }

        internal User(ProtoUser proto)
        {
            ProtoObject = proto ?? new ProtoUser();
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            roles = new ObservableCollection<int>(ProtoObject.Roles);
            roles.CollectionChanged += OnRolesCollectionChanged;
        }

        #region ProtoFields

        internal ProtoUser ProtoObject { get; init; }

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

        public string Password
        {
            get => ProtoObject.Password;
            set
            {
                ProtoObject.Password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        private ObservableCollection<int> roles;
        public ObservableCollection<int> Roles
        {
            get => roles;
            set
            {
                if (roles != null)
                    roles.CollectionChanged -= OnRolesCollectionChanged;

                roles = value ?? new ObservableCollection<int>();
                roles.CollectionChanged += OnRolesCollectionChanged;

                SyncRoles();
                RaisePropertyChanged(nameof(Roles));
            }
        }

        private void OnRolesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncRoles();
            RaisePropertyChanged(nameof(Roles));
        }

        private void SyncRoles()
        {
            ProtoObject.Roles.Clear();
            ProtoObject.Roles.AddRange(roles);
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
