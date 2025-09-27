using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrpcServiceClient.DataContracts
{
    

    public sealed partial class BackupStatusResponse : INotifyPropertyChanged
    {
        #region ProtoFields

        internal ProtoBackupStatusResponse ProtoObject { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Конструкторы

        public BackupStatusResponse()
        {
            ProtoObject = new ProtoBackupStatusResponse();
        }

        internal BackupStatusResponse(ProtoBackupStatusResponse proto)
        {
            ProtoObject = proto ?? new ProtoBackupStatusResponse();
        }

        #endregion

        #region Свойства

        public bool IsDone
        {
            get => ProtoObject.IsDone;
            set
            {
                ProtoObject.IsDone = value;
                RaisePropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => ProtoObject.ErrorMessage;
            set
            {
                ProtoObject.ErrorMessage = value;
                RaisePropertyChanged();
            }
        }

        public int FileSizeIn8KbChunks
        {
            get => ProtoObject.FileSizeIn8KbChunks;
            set
            {
                ProtoObject.FileSizeIn8KbChunks = value;
                RaisePropertyChanged();
            }
        }

        public bool IsInProcess
        {
            get => ProtoObject.IsInProcess;
            set
            {
                ProtoObject.IsInProcess = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Вспомогательные методы

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
