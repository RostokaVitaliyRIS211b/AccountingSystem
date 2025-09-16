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
    public interface IUnitTypeSelectorViewModel
    {
        public TypeOfUnit? SelectedUnit { get; set; }
        public ObservableCollection<TypeOfUnit> Units { get; set; }
        public string FilterUnit { get; set; }

        public Window? Win { get; set; }
    }
}
