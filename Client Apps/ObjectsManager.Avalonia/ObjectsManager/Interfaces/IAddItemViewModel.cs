using Avalonia.Controls;

using GrpcServiceClient.DataContracts;

using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IAddItemViewModel
    {
        public ItemWrapper Wrapper { get; }
        public ObservableCollection<TypeOfItem> TypeOfItems { get; set; }
        public Window? Win { get; set; }

        public abstract Task EditGroupingProperties();
        public abstract Task OpenProducerSelectorWindow();
        public abstract Task OpenTypeOfUnitSelector();
        public abstract Task OpenNameSelectorWindow();
        public abstract Task ApplyChanges();
    }
}
