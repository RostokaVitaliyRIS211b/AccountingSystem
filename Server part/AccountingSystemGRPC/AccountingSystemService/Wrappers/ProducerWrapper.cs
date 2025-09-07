using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class ProducerWrapper : INotifyPropertyChanged
    {
        public ProducerWrapper()
        {
            ProtoObject = new ProtoProducer();
        }

        internal ProducerWrapper(ProtoProducer proto)
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
