using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrpcServiceClient.DataContracts
{
    

    public sealed partial class BackupChunk : INotifyPropertyChanged
    {
        #region ProtoFields

        internal ProtoBackupChunk ProtoObject { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Конструкторы

        public BackupChunk()
        {
            ProtoObject = new ProtoBackupChunk();
        }

        internal BackupChunk(ProtoBackupChunk proto)
        {
            ProtoObject = proto ?? new ProtoBackupChunk();
        }

        #endregion

        #region Свойства

        public bool IsLast
        {
            get => ProtoObject.IsLast;
            set
            {
                ProtoObject.IsLast = value;
                RaisePropertyChanged();
            }
        }

        public byte[] Data
        {
            get => ProtoObject.Data?.ToByteArray() ?? [];
            set
            {
                ProtoObject.Data = Google.Protobuf.ByteString.CopyFrom(value);
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
