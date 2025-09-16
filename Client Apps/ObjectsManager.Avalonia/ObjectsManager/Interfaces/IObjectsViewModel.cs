using GrpcServiceClient.DataContracts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IObjectsViewModel
    {
        public string SelectedObjName { get; set; }
        public ConObject? SelectedObj { get; set; }
        public ObservableCollection<ConObject> FilteredConObjects { get; }
        public string FilterObj { get; set; }

        public abstract Task AddObject();

        public abstract Task RemoveObject();
    }
}
