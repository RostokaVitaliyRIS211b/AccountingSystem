using Avalonia.Controls;

using GrpcServiceClient;

using MsBox.Avalonia;

using ObjectsManager.Core;
using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public partial class ProgressBarViewModel : ViewModelBase, IProgressBarViewModel
    {
        public ProgressBarViewModel(MainService service)
        {
            Service = service;
        }

        private MainService Service { get; set; }

        private string _mess = "";
        public string Message { get => _mess; set { _mess = value; OnPropertyChanged(nameof(Message)); } }


        private double _val = 0;
        public double Value { get => _val; set { _val = value; OnPropertyChanged(nameof(Value)); } }


        private double _max = double.MaxValue;
        public double Maximum { get => _max; set { _max = value; OnPropertyChanged(nameof(Maximum)); } }


        private string _head = "Бэкап базы данных";
        public string Header { get => _head; set { _head = value; OnPropertyChanged(nameof(Header)); } }

        public Window? Win { get; set; }

        public Interaction<object?, string?> Save { get; } = new();

        public bool StopBackup { get; set; } = false;

        public bool IsBackupInProgress { get; set; } = false;

        private string FilePath { get; set; } = "";

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        public async Task StartBackup()
        {
            try
            {
                if (IsBackupInProgress)
                {
                    return;
                }

                using var cts = new CancellationTokenSource();
                IsBackupInProgress = true;
                var path = await Save.HandleAsync(null);

                if (string.IsNullOrWhiteSpace(path))
                {
                    IsBackupInProgress = false;
                    throw new Exception($"Путь до файла бэкапа не может быть пустым");
                }
                else
                {
                    Value = 0;
                    FilePath = path;
                    using var file = File.Create(path);
                    Message = "Создание бэкапа на сервере";
                    await foreach (var chunk in Service.StartBackupAsync(cts.Token))
                    {
                        await file.WriteAsync(chunk);

                        if (Maximum == double.MaxValue)
                        {
                            var status = await Service.GetBackupStatusAsync();
                            if (!string.IsNullOrWhiteSpace(status.ErrorMessage))
                            {
                                cts.Cancel();
                                throw new Exception(status.ErrorMessage);
                            }
                            else
                            {
                                if(status.FileSizeIn8KbChunks > 0)
                                {
                                    Maximum = status.FileSizeIn8KbChunks;
                                    Message = "Загрузка бэкапа";
                                }
                            }
                        }
                        else
                        {
                            ++Value;
                        }

                        if (StopBackup)
                        {
                            cts.Cancel();
                            throw new Exception("Бэкап остановлен пользователем");
                        }
                    }

                    var status2 = await Service.GetBackupStatusAsync();
                    if (!string.IsNullOrWhiteSpace(status2.ErrorMessage))
                    {
                        throw new Exception(status2.ErrorMessage);
                    }
                    else
                    {
                        Message = "Бэкап успешно загружен!";

                        await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetSuccessBoxParams($"Бэкап успешно загружен!")).ShowAsync();
                    }
                }
            }
            catch (Exception e)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageBoxParamsHelper.GetErrorBoxParams($"Ошибка при бэкапе базы данных -> {e.Message}")).ShowAsync();
                if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                {
                    try
                    {
                        File.Delete(FilePath);
                    }
                    catch
                    {

                    }
                }
            }
            finally
            {
                IsBackupInProgress = false;
                Win?.Close();
            }
        }

        async Task IProgressBarViewModel.StopBackup()
        {
            if (IsBackupInProgress)
            {
                var res = await MessageBoxManager.GetMessageBoxCustom(MessageBoxParamsHelper.GetYesOrNoQuestionBoxParams("Вопрос", "Остановить бэкап?")).ShowAsync();
                if (res == "Да")
                {
                    StopBackup = true;
                }
            }
            else
            {
                Win?.Close();
            }
        }
    }
}
