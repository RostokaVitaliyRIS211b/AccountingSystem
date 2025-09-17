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
    public partial class UsersViewModel : ViewModelBase, IUsersViewModel
    {
        public UsersViewModel(MainService service)
        {
            Service = service;
            var usersList = Service.GetAllUsers();
            foreach (var item in usersList)
            {
                UsersCollection.Add(item);
            }
            var roleList = Service.GetAllRoles();
            foreach (var item in roleList)
            {
                RolesCollection.Add(new CheckBoxRole() { SpermRole = item });
            }
        }
        public Window? Win { get; set; }
        MainService Service { get; set; }
        public ObservableCollection<User> UsersCollection { get; } = [];
        public ObservableCollection<CheckBoxRole> RolesCollection { get; } = [];

        private User? _user;
        public User? SelectedUser { get => _user; set { _user = value; OnPropertyChanged(nameof(SelectedUser)); } }
        public void UpdateCheckBox()
        {
            foreach (CheckBoxRole role in RolesCollection)
            {
                role.PropertyChanged -= Role_PropertyChanged;
                role.IsAllowed = SelectedUser?.Roles.Contains(role.SpermRole.Id) ?? false;
                role.PropertyChanged += Role_PropertyChanged;
            }
        }

        private void Role_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RoleChanged();
        }

        public void RoleChanged()
        {
            try
            {
                foreach (var role in RolesCollection.Where(sperm => sperm.IsAllowed))
                {
                    if (!SelectedUser.Roles.Contains(role.SpermRole.Id))
                        SelectedUser.Roles.Add(role.SpermRole.Id);
                }
                foreach (var role in RolesCollection.Where(sperm => !sperm.IsAllowed))
                {
                    if (SelectedUser.Roles.Contains(role.SpermRole.Id))
                        SelectedUser.Roles.Remove(role.SpermRole.Id);
                }
                if (Service.UpdateUser(SelectedUser))

                    MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Изменение роли пользователя прошло успешно!")).ShowAsync();
                else

                    MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Не удалось изменить роль пользователя!")).ShowAsync();
            }
            catch (Exception e)
            {
                MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при попытке изменить разрешения роли {SelectedUser?.Name} -> {e.Message}")).ShowAsync();
            }

        }
        [RelayCommand]
        public async Task AddUser()
        {
            try
            {
                var newUser = new User();
                newUser.Name = $"Новый пользователь {DateTime.Now}";
                newUser.Id = await Service.AddUserAsync(newUser);
                if (newUser.Id == -1)
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Пользователь не был создан")).ShowAsync();
                    return;
                }
                UsersCollection.Add(newUser);
            }
            catch (SystemException e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при создании новой пользователя -> {e.Message}")).ShowAsync();
            }
        }
        [RelayCommand]
        public async Task DelUser()
        {
            if (SelectedUser == null) return;
            try
            {
                var res = await MessageBoxManager.GetMessageBoxCustom(MessageBoxParamsHelper.GetYesOrNoQuestionBoxParams("Вопрос", $"Вы точно хотите удалить пользователя {SelectedUser.Name}?")).ShowAsync();
                if (res == "Да")
                {
                    await Service.RemoveUserAsync(SelectedUser);
                    UsersCollection?.Remove(SelectedUser);
                }
            }
            catch (SystemException e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при удалении пользователя -> {e.Message}")).ShowAsync();
            }
        }
        public void SaveChanges()
        {
            try
            {
                if (SelectedUser == null)
                {
                    return;
                }
                Service.UpdateUser(SelectedUser);
            }
            catch (Exception e)
            {

                MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Не удалось изменить пользователя -> {e.Message}")).ShowAsync();
            }

        }
    }
    public class CheckBoxRole : INotifyPropertyChanged
    {
        public required Role SpermRole { get; set; }
        private bool _IsAllowed;
        public bool IsAllowed { get => _IsAllowed; set { _IsAllowed = value; OnPropertyChanged(nameof(IsAllowed)); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
        }
    }
}
