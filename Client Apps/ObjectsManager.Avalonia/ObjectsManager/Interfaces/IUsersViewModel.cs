using Avalonia.Controls;
using GrpcServiceClient.DataContracts;
using ObjectsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IUsersViewModel
    {
        public Window? Win { get; set; }
        public ObservableCollection<User> UsersCollection { get; }
        public ObservableCollection<CheckBoxRole> RolesCollection { get; }

        public User? SelectedUser { get; set; }
        public abstract Task AddUser();
        public abstract Task DelUser();
        public void SaveChanges();
        public void UpdateCheckBox();
        public void RoleChanged();
    }
}
