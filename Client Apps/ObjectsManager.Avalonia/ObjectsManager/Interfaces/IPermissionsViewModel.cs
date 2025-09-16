using Avalonia.Controls;
using GrpcServiceClient.DataContracts;
using ObjectsManager.Helpers;
using ObjectsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IPermissionsViewModel
    {
        public ObservableCollection<CheckBoxPermission> PermissionsCollection { get; }
        public Window? Win { get; set; }
        public Role SelectedRole { get; set; }
        public abstract Task SavePermissions();
    }
}
