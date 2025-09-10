using AccountingSystemService;

using System.ComponentModel;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class GroupingProperty : INotifyPropertyChanged
    {
        public GroupingProperty()
        {
            ProtoObject = new ProtoGroupingProperty();
        }

        internal GroupingProperty(ProtoGroupingProperty proto)
        {
            ProtoObject = proto ?? new ProtoGroupingProperty();
        }

        #region ProtoFields

        internal ProtoGroupingProperty ProtoObject { get; init; }

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
