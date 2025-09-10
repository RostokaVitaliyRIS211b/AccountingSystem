using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class JournalRecord : INotifyPropertyChanged
    {
        public JournalRecord()
        {
            ProtoObject = new ProtoJournalRecord();
        }

        internal JournalRecord(ProtoJournalRecord proto)
        {
            ProtoObject = proto ?? new ProtoJournalRecord();
        }

        #region ProtoFields

        internal ProtoJournalRecord ProtoObject { get; init; }

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

        public string Text
        {
            get => ProtoObject.Text;
            set
            {
                ProtoObject.Text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
