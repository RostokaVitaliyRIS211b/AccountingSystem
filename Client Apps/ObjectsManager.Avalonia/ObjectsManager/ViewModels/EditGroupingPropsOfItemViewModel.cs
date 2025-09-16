using Avalonia.Controls;

using GrpcServiceClient;
using GrpcServiceClient.DataContracts;

using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    

    public class EditGroupingPropsOfItemViewModel:ViewModelBase,IEditGroupingPropsOfItemViewModel
    {
        [Obsolete]
        public EditGroupingPropsOfItemViewModel()
        {
            throw new NotImplementedException();
        }

        public EditGroupingPropsOfItemViewModel(ItemWrapper wrapper, MainService service, ref ObservableCollection<GroupingProperty> properties)
        {
            
        }

        public Window? Win { get; set; }
    }
}
