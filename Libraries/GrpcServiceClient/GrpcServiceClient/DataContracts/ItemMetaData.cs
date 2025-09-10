using Google.Protobuf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class ItemMetaData : INotifyPropertyChanged
    {
        public ItemMetaData()
        {
            ProtoObject = new ProtoItemMetaData();
        }

        internal ItemMetaData(ProtoItemMetaData proto)
        {
            ProtoObject = proto ?? new ProtoItemMetaData();
        }

        #region ProtoFields

        internal ProtoItemMetaData ProtoObject { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => ProtoObject.Id;
            set
            {
                ProtoObject.Id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => ProtoObject.Name;
            set
            {
                ProtoObject.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public int ItemId
        {
            get => ProtoObject.ItemId;
            set
            {
                ProtoObject.ItemId = value;
                RaisePropertyChanged(nameof(ItemId));
            }
        }

        public byte[] Data
        {
            get => ProtoObject.Data?.ToArray() ?? Array.Empty<byte>();
            set
            {
                ProtoObject.Data = ByteString.CopyFrom(value);
                RaisePropertyChanged(nameof(Data));
            }
        }

        public int TypeId
        {
            get => ProtoObject.TypeId;
            set
            {
                ProtoObject.TypeId = value;
                RaisePropertyChanged(nameof(TypeId));
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
