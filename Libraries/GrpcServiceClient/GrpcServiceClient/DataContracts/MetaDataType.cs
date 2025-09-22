using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class MetaDataType : INotifyPropertyChanged
    {
        public MetaDataType()
        {
            ProtoObject = new ProtoMetaDataType();
        }

        internal MetaDataType(ProtoMetaDataType proto)
        {
            ProtoObject = proto ?? new ProtoMetaDataType();
        }

        #region ProtoFields

        internal ProtoMetaDataType ProtoObject { get; init; }

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

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
