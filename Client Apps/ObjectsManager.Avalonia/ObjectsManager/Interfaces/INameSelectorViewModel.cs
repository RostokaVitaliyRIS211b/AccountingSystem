using Avalonia.Controls;

using GrpcServiceClient.DataContracts;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface INameSelectorViewModel
    {
        public NameItem? SelectedName { get; set; }
        public ObservableCollection<NameItem> Names { get; set; }
        public string FilterName { get; set; }

        public Window? Win { get; set; }
    }
}
