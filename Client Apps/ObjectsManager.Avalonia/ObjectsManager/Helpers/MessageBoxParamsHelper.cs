using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ObjectsManager.Helpers
{
    internal static class MessageBoxParamsHelper
    {
        public static WindowIcon IconOfWindow;

        static MessageBoxParamsHelper()
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            IconOfWindow = new WindowIcon(new Bitmap(AssetLoader.Open(new("avares://ObjectsManager/Assets/ColorIcons/icons8-home-50.png"))));
        }

        public static MessageBoxStandardParams GetSuccessBoxParams(string message)
        {
            return new MessageBoxStandardParams()
            {
                ContentTitle = "Информация",
                ButtonDefinitions = ButtonEnum.Ok,
                ContentMessage = message,
                Icon = Icon.Success,
                WindowIcon = IconOfWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MinWidth = 300,
                MaxWidth = 400
            };
        }

        public static MessageBoxCustomParams GetYesOrNoQuestionBoxParams(string title,string question)
        {
            return new MessageBoxCustomParams()
            {
                ContentTitle = title,
                ContentMessage = question,
                ButtonDefinitions = [new ButtonDefinition() { Name = "Да", IsCancel = true }, new ButtonDefinition() { Name = "Нет", IsDefault = true }],
                Icon = Icon.Question,
                WindowIcon = IconOfWindow,
                MinWidth = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MaxWidth = 400
            };
        }

        public static MessageBoxStandardParams GetInfoBoxParams(string infoMessage)
        {
            return new MessageBoxStandardParams()
            {
                ContentTitle = "Информация",
                ButtonDefinitions = ButtonEnum.Ok,
                ContentMessage = infoMessage,
                Icon = Icon.Info,
                WindowIcon = IconOfWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MinWidth = 300,
                MaxWidth = 400
            };
        }

        public static MessageBoxStandardParams GetErrorBoxParams(string errorMessage)
        {
            return new MessageBoxStandardParams()
            {
                ContentTitle = "Ошибка",
                ButtonDefinitions = ButtonEnum.Ok,
                ContentMessage = errorMessage,
                Icon = Icon.Error,
                WindowIcon = IconOfWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MinWidth = 300,
                MaxWidth = 320,
                CanResize = false
            };
        }

        public static MessageBoxStandardParams GetWarningBoxParams(string warningMessage)
        {
            return new MessageBoxStandardParams()
            {
                ContentTitle = "Предупреждение",
                ButtonDefinitions = ButtonEnum.Ok,
                ContentMessage = warningMessage,
                Icon = Icon.Warning,
                WindowIcon = IconOfWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                MinWidth = 300,
                MaxWidth = 320,
                CanResize = false
            };
        }
    }
}
