using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using GrpcServiceClient;
using GrpcServiceClient.DataContracts;
using MsBox.Avalonia;
using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;
using ObjectsManager.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class RolesViewModel : ViewModelBase, IRolesViewModel
    {
        public RolesViewModel(MainService service) 
        {
            Service = service;
            var rolesList = Service.GetAllRoles();
            foreach (var item in rolesList)
            {
                RolesCollection.Add(item);
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveChanges();
        }

        private MainService Service { get; set; }
        public ObservableCollection<Role> RolesCollection { get; } = [];
        public Window? Win { get; set; }

        private Role? _role;
        public Role? SelectedRole { get => _role; set { _role = value; OnPropertyChanged(nameof(SelectedRole)); } }

        [RelayCommand]
        public async Task AddRoles()
        {
            try
            {
                var newRole = new Role();
                newRole.Name = $"Новая роль {DateTime.Now}";
                newRole.Id = await Service.AddRoleAsync(newRole);
                if (newRole.Id == -1)
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Роль не была создана")).ShowAsync();
                    return;
                }
                RolesCollection.Add(newRole);
                newRole.PropertyChanged += Item_PropertyChanged;
            }
            catch(SystemException e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при создании новой роли -> {e.Message}")).ShowAsync();
            }
        }

        [RelayCommand]
        public async Task DelRoles()
        {
            if (SelectedRole == null) return;
            try
            {
                var res = await MessageBoxManager.GetMessageBoxCustom(MessageBoxParamsHelper.GetYesOrNoQuestionBoxParams("Вопрос", $"Вы точно хотите удалить роль {SelectedRole.Name}?")).ShowAsync();
                if (res == "Да")
                {
                    await Service.RemoveRoleAsync(SelectedRole);
                    RolesCollection?.Remove(SelectedRole);
                }
            }
            catch (SystemException e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении роли -> {e.Message}")).ShowAsync();
            }
        }

        [RelayCommand]
        public async Task SetPermissions()
        {
            try
            {
                if (SelectedRole == null) return;
                var permissionsWindow = new PermissionsWindow(new PermissionsViewModel(SelectedRole, Service));
                permissionsWindow.Show();
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при открытии окна Разрешений -> {e.Message}")).ShowAsync();
            }
        }

        public void SaveChanges()
        {
            try
            {
                if (SelectedRole == null)
                {
                    return;
                }
                Service.UpdateRole(SelectedRole);
            }
            catch (Exception e)
            {
                MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Не удалось изменить роль -> {e.Message}")).ShowAsync();
            }

        }
    }
}
