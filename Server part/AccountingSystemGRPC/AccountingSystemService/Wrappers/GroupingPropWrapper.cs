using System.ComponentModel;

namespace AccountingSystemService.Wrappers
{
    public sealed partial class GroupingPropertyWrapper : INotifyPropertyChanged
    {
        public GroupingPropertyWrapper()
        {
            ProtoObject = new ProtoGroupingProperty();
        }

        internal GroupingPropertyWrapper(ProtoGroupingProperty proto)
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
