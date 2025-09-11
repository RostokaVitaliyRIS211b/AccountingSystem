using Avalonia.Controls;

using CommunityToolkit.Mvvm.Input;

using MsBox.Avalonia;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class AuthorizationViewModel : ViewModelBase, IAuthorizationViewModel
    {
        private string _name = "";
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        private string _pwd = "";
        public string Password { get => _pwd; set { _pwd = value; OnPropertyChanged(nameof(Password)); } }

        private string _ip = "";
        public string IpAddress { get => _ip; set { _ip = value; OnPropertyChanged(nameof(Name)); } }

        private string _port = "";
        public string Port { get => _port; set { _port = value; OnPropertyChanged(nameof(Name)); } }
        public Window? win { get; set; }

        [RelayCommand]
        public async Task Authorize()
        {

            try
            {

            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при авторизации -> {e.Message}")).ShowAsync();
            }

        }
    }
}
