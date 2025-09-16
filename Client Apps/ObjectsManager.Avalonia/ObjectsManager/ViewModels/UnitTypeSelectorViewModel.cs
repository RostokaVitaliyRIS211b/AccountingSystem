using Avalonia.Controls;

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
    public class UnitTypeSelectorViewModel:ViewModelBase,IUnitTypeSelectorViewModel
    {
        [Obsolete]
        public UnitTypeSelectorViewModel()
        {
            throw new NotImplementedException();
        }

        public UnitTypeSelectorViewModel(IEnumerable<TypeOfUnit> nameItems)
        {
            Units = [.. nameItems];
            AllUnits = [.. nameItems];
        }

        private List<TypeOfUnit> AllUnits { get; } = [];

        private TypeOfUnit? _unit = null;
        public TypeOfUnit? SelectedUnit { get => _unit; set { _unit = value; OnPropertyChanged(nameof(SelectedUnit)); } }

        public ObservableCollection<TypeOfUnit> Units { get; set; } = [];


        private string _filter = "";
        public string FilterUnit { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterUnit)); Filter(); } }

        public Window? Win { get; set; }

        private void Filter()
        {
            Units.Clear();
            foreach (var item in AllUnits.Where(x => x.Name.Contains(FilterUnit, StringComparison.OrdinalIgnoreCase)))
            {
                Units.Add(item);
            }
        }
    }
}
