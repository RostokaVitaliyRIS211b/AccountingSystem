using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class Producer : INotifyPropertyChanged
    {
        public Producer()
        {
            ProtoObject = new ProtoProducer();
        }

        internal Producer(ProtoProducer proto)
        {
            ProtoObject = proto ?? new ProtoProducer();
        }

        #region ProtoFields

        internal ProtoProducer ProtoObject { get; init; }

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
