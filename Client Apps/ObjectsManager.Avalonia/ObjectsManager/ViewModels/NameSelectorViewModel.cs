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
    public class NameSelectorViewModel : ViewModelBase, INameSelectorViewModel
    {
        [Obsolete]
        public NameSelectorViewModel()
        {
            throw new NotImplementedException();
        }

        public NameSelectorViewModel(IEnumerable<NameItem> nameItems)
        {
            Names = [.. nameItems];
            AllNames = [.. nameItems];
        }

        private List<NameItem> AllNames { get; } = [];

        private NameItem? _name = null;
        public NameItem? SelectedName { get => _name; set { _name = value; OnPropertyChanged(nameof(SelectedName)); } }

        public ObservableCollection<NameItem> Names { get; set; } = [];


        private string _filter = "";
        public string FilterName { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterName)); Filter(); } }

        public Window? Win { get; set; }

        private void Filter()
        {
            Names.Clear();
            foreach(var item in AllNames.Where(x=>x.Name.Contains(FilterName,StringComparison.OrdinalIgnoreCase)))
            {
                Names.Add(item);
            }
        }
    }
}
