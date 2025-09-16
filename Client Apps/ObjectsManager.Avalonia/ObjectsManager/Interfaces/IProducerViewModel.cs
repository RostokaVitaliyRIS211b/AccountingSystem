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
    public interface IProducerViewModel
    {
        public Producer? SelectedProducer { get; set; }
        public ObservableCollection<Producer> Producers { get; set; }
        public string FilterProducer { get; set; }

        public Window? Win { get; set; }
    }
}
