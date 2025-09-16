using Avalonia.Controls;
using Avalonia.Styling;

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
    public interface IMainViewModel
    {
        public string SelectedObjName { get; set; }
        public ConObject? SelectedObj { get; set; }
        public Item? SelectedObjItem { get; set; }
        public ObservableCollection<ConObject> ConObjects { get; }
        public ObservableCollection<ConObject> FilteredConObjects { get; }
        public string FilterObj { get; set; }
        public abstract Task OpenRoleWindow();
        public abstract Task OpenObjectsWindow();

        public ThemeVariant Light { get; }

        public ThemeVariant Dark { get; }

        public Window? Win { get; set; }

        public decimal? NumberOfGroups { get; set; }

        public ObservableCollection<ItemWrapper> ItemsOfConObj { get; set; }

        public abstract Task AddItem();
    }
}
