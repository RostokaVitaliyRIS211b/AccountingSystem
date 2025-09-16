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
    public class ProducerSelectorViewModel:ViewModelBase, IProducerViewModel
    {
        [Obsolete]
        public ProducerSelectorViewModel()
        {
            throw new NotImplementedException();
        }

        public ProducerSelectorViewModel(IEnumerable<Producer> nameItems)
        {
            Producers = [.. nameItems];
            AllProducers = [.. nameItems];
        }

        private List<Producer> AllProducers { get; } = [];

        private Producer? _producer = null;
        public Producer? SelectedProducer { get => _producer; set { _producer = value; OnPropertyChanged(nameof(SelectedProducer)); } }

        public ObservableCollection<Producer> Producers { get; set; } = [];


        private string _filter = "";
        public string FilterProducer { get => _filter; set { _filter = value; OnPropertyChanged(nameof(FilterProducer)); Filter(); } }

        public Window? Win { get; set; }

        private void Filter()
        {
            Producers.Clear();
            foreach (var item in AllProducers.Where(x => x.Name.Contains(FilterProducer, StringComparison.OrdinalIgnoreCase)))
            {
                Producers.Add(item);
            }
        }
    }
}
