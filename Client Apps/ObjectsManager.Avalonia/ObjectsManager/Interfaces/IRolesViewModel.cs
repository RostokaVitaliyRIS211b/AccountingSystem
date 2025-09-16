using Avalonia.Controls;
using GrpcServiceClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IRolesViewModel
    {
        public Window? Win { get; set; }
        public ObservableCollection<Role> RolesCollection { get; }
        public abstract Task SetPermissions();
        public abstract Task AddRoles();
        public abstract Task DelRoles();
        public Role? SelectedRole { get; set; }
        public void SaveChanges();
    }
}