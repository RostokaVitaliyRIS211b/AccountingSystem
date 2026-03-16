using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
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
using System.Threading;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class AuthorizationViewModel : ViewModelBase, IAuthorizationViewModel
    {
        public AuthorizationViewModel()
        {
            Name = AppSettingsHelper.Settings.UserName;
            IpAddress = AppSettingsHelper.Settings.IpAddress;
            Port = AppSettingsHelper.Settings.Port;
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


        private string _LoadingText = "Загрузка";
        public string LoadingText { get => _LoadingText; set { _LoadingText = value; OnPropertyChanged(nameof(LoadingText)); } }


        private bool _IsAuthorizeInProcess = false;
        public bool IsAuthorizeInProcess { get => _IsAuthorizeInProcess; set { _IsAuthorizeInProcess = value; OnPropertyChanged(nameof(IsAuthorizeInProcess)); } }


        [RelayCommand]
        public async Task Authorize()
        {
            var tokenSource = new CancellationTokenSource();
            try
            {
                await Task.Delay(1);
                Dispatcher.UIThread.Invoke(() => { IsAuthorizeInProcess = true; });
                _ = StartLoadingAnimation(tokenSource.Token);
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
                //var speed = 100 * 1024*1024;

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
                    AppSettingsHelper.SaveSettings(new(IpAddress, Port, Name, pwd,AppSettingsHelper.Settings.IsCachingOn));
                }
                catch
                {

                }

                if (Application.Current != null)
                {
                    var window = new MainWindow(new MainViewModel(service));

                    if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
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
            finally
            {
                IsAuthorizeInProcess = false;
                tokenSource.Cancel();
            }

        }

        [GeneratedRegex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")]
        private static partial Regex IpValidation();

        public async Task StartLoadingAnimation(CancellationToken token)
        {
            Dispatcher.UIThread.Invoke(() => { LoadingText = "Загрузка"; });
            var delay = 300;
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(delay, token);
                Dispatcher.UIThread.Invoke(() => { LoadingText = LoadingText + "."; });
                await Task.Delay(delay, token);
                Dispatcher.UIThread.Invoke(() => { LoadingText = LoadingText + "."; });
                await Task.Delay(delay, token);
                Dispatcher.UIThread.Invoke(() => { LoadingText = LoadingText + "."; });
                await Task.Delay(delay, token);
                Dispatcher.UIThread.Invoke(() => { LoadingText = "Загрузка"; });
            }
        }
    }
}
