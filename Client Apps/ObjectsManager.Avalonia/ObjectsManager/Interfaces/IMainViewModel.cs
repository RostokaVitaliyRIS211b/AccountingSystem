using GrpcServiceClient.DataContracts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IMainViewModel
    {
        public string SelectedObjName { get; set; }
        public ConObject? SelectedObj { get; set; }
        public Item? SelectedObjItem { get; set; }
        public ObservableCollection<ConObject> ConObjects { get; }
        public string FilterObj { get; set; }
        public abstract Task OpenRoleWindow();
    }
}
