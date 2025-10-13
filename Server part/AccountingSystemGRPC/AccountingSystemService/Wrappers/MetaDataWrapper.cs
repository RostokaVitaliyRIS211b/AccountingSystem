using Google.Protobuf;
using BdClasses;
using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class MetaDataWrapper : INotifyPropertyChanged
    {
        public MetaDataWrapper()
        {
            ProtoObject = new ProtoItemMetaData();
        }

        internal MetaDataWrapper(ProtoItemMetaData proto)
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
            get => ProtoObject.Data?.ToByteArray() ?? Array.Empty<byte>();
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
