using Google.Protobuf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class ObjectMetadata : INotifyPropertyChanged
    {
        public ObjectMetadata()
        {
            ProtoObject = new ProtoObjectMetadata();
        }

        internal ObjectMetadata(ProtoObjectMetadata proto)
        {
            ProtoObject = proto ?? new ProtoObjectMetadata();
        }

        #region ProtoFields

        internal ProtoObjectMetadata ProtoObject { get; init; }

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

        public int ObjId
        {
            get => ProtoObject.ObjId;
            set
            {
                ProtoObject.ObjId = value;
                RaisePropertyChanged(nameof(ObjId));
            }
        }

        public byte[] Data
        {
            get => ProtoObject.Data?.ToByteArray() ?? [];
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
