using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using CommunityToolkit.Mvvm.Input;

using GrpcServiceClient;

using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;
using ObjectsManager.Windows;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class AuthorizationViewModel : ViewModelBase, IAuthorizationViewModel
    {
        public AuthorizationViewModel()
        {
            Name = ConnectionSettingsHelper.Settings.UserName;
            IpAddress = ConnectionSettingsHelper.Settings.IpAddress;
            Port = ConnectionSettingsHelper.Settings.Port;
        }

        private string _name = "";
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        private string _pwd = "";
        public string Password { get => _pwd; set { _pwd = value; OnPropertyChanged(nameof(Password)); } }

        private string _ip = "";
        public string IpAddress { get => _ip; set { _ip = value; OnPropertyChanged(nameof(Name)); } }

        private string _port = "";
        public string Port { get => _port; set { _port = value; OnPropertyChanged(nameof(Name)); } }
        public Window? Win { get; set; }

        [RelayCommand]
        public async Task Authorize()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Имя пользователя не может быть пустым")).ShowAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Пароль пользователя не может быть пустым")).ShowAsync();
                    return;
                }

                if (!IpValidation().IsMatch(IpAddress))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams("Введенный IP адрес является неправильным")).ShowAsync();
                    return;
                }

                if (!int.TryParse(Port, out int res))
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams("Введенный порт является неправильным")).ShowAsync();
                    return;
                }

                var pwd = StringCipher.Encrypt(Password);

                MainService service = new(Name, pwd, $"http://{IpAddress}:{Port}");

                try
                {
                    await service.CheckActiveAsync();
                }
                catch
                {
                    await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Неправильный логин или пароль")).ShowAsync();
                    return;
                }

                try
                {
                    ConnectionSettingsHelper.SaveSettings(new(IpAddress, Port, Name, pwd));
                }
                catch
                {

                }

                if(Application.Current != null)
                {
                    var window = new MainWindow(new MainViewModel(service));

                    if(Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                    {
                        lifetime.MainWindow = window;
                    }

                    window.Show();

                    Win?.Close();
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при авторизации -> {e.Message}")).ShowAsync();
            }

        }

        [GeneratedRegex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")]
        private static partial Regex IpValidation();
    }
}
