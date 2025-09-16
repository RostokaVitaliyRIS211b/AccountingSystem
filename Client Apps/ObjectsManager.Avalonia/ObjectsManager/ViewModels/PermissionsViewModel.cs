using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using GrpcServiceClient;
using GrpcServiceClient.DataContracts;
using MsBox.Avalonia;
using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class PermissionsViewModel : ViewModelBase, IPermissionsViewModel
    {
        public PermissionsViewModel(Role selectedRole, MainService service)
        {
            SelectedRole = selectedRole;
            Service = service;
            var permissionsList = service.GetAllPermissions();
            foreach (var item in permissionsList)
            {

                PermissionsCollection.Add(new CheckBoxPermission() { SpermPermission = item });
            }
            UpdateCheckboxes();
        }



        private MainService Service { get; set; }
        public ObservableCollection<CheckBoxPermission> PermissionsCollection { get; } = [];
        public Window? Win { get; set; }

        private Role _role;
        public Role SelectedRole { get => _role; set { _role = value; OnPropertyChanged(nameof(SelectedRole)); } }

        

        private void UpdateCheckboxes()
        {
            foreach (CheckBoxPermission permission in PermissionsCollection)
            {
                permission.IsAllowed = SelectedRole.Permissions.Contains(permission.SpermPermission.Id);
            }
        }



        [RelayCommand]
        public async Task SavePermissions()
        {

            try
            {
                foreach (var permission in PermissionsCollection.Where(sperm => sperm.IsAllowed))
                {
                    if (!SelectedRole.Permissions.Contains(permission.SpermPermission.Id))
                        SelectedRole.Permissions.Add(permission.SpermPermission.Id);
                }
                foreach (var permission in PermissionsCollection.Where(sperm => !sperm.IsAllowed))
                {
                    if (SelectedRole.Permissions.Contains(permission.SpermPermission.Id))
                        SelectedRole.Permissions.Remove(permission.SpermPermission.Id);
                }
                if (await Service.UpdateRoleAsync(SelectedRole))

                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Изменение роли прошло успешно!")).ShowAsync();
                else

                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Не удалось изменить разрешения роли!")).ShowAsync();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при попытке изменить разрешения роли {SelectedRole?.Name} -> {e.Message}")).ShowAsync();
            }

        }
    }



    public class CheckBoxPermission : INotifyPropertyChanged
    {
        public required Permission SpermPermission { get; set; }
        private bool _IsAllowed;
        public bool IsAllowed { get => _IsAllowed; set { _IsAllowed = value; OnPropertyChanged(nameof(IsAllowed)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
        }
    }
}