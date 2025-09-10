using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcServiceClient.DataContracts
{
    public sealed partial class Journal : INotifyPropertyChanged
    {
        public Journal()
        {
            ProtoObject = new ProtoJournal();
            InitializeCollections();
        }

        internal Journal(ProtoJournal proto)
        {
            ProtoObject = proto ?? new ProtoJournal();
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            records = new ObservableCollection<JournalRecord>(
                ProtoObject.Records.Select(r => new JournalRecord(r))
            );
            records.CollectionChanged += OnRecordsCollectionChanged;
        }

        #region ProtoFields

        internal ProtoJournal ProtoObject { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<JournalRecord> records;
        public ObservableCollection<JournalRecord> Records
        {
            get => records;
            set
            {
                if (records != null)
                    records.CollectionChanged -= OnRecordsCollectionChanged;

                records = value ?? new ObservableCollection<JournalRecord>();
                records.CollectionChanged += OnRecordsCollectionChanged;

                SyncRecords();
                RaisePropertyChanged(nameof(Records));
            }
        }

        private void OnRecordsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SyncRecords();
            RaisePropertyChanged(nameof(Records));
        }

        private void SyncRecords()
        {
            ProtoObject.Records.Clear();
            ProtoObject.Records.AddRange(records.Select(r => r.ProtoObject));
        }
        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
