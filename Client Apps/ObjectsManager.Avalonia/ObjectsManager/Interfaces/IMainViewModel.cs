using Avalonia.Controls;
using Avalonia.Styling;

using GrpcServiceClient.DataContracts;

using ObjectsManager.Core;
using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IMainViewModel:IDisposable
    {
        public string SelectedObjName { get; set; }
        public ConObject? SelectedObj { get; set; }
        public ItemWrapper? SelectedObjItem { get; set; }
        public ObservableCollection<ConObject> ConObjects { get; }
        public ObservableCollection<ConObject> FilteredConObjects { get; }

        public ObservableCollection<ItemWrapper> SelectedItemsOfConObj { get; set; }
        public string FilterObj { get; set; }
        public abstract Task OpenRoleWindow();
        public abstract Task OpenObjectsWindow();

        public ThemeVariant Light { get; }

        public ThemeVariant Dark { get; }

        public Window? Win { get; set; }

        public decimal? NumberOfGroups { get; set; }

        public ObservableCollection<ItemWrapper> ItemsOfConObj { get; set; }

        public abstract Task AddItem();

        public abstract Task EditItem();

        public abstract Task EditGroupingPropertiesOfItem();

        public abstract Task DeleteItem();

        public abstract Task AddGroupingPropToSelectedItems();

        public abstract Task DeleteGroupingPropOfSelectedItems();

        public abstract Task SetGroupingPropsOfSelectedItems();

        public abstract bool FilteringItem(object item);

        public Func<IEnumerable<ItemWrapper>>? GetSelectedItems { get; set; }

        public Action? FilterGrid { get; set; }

        public Interaction<object?, string?> LoadItems { get; } 

        public Interaction<object?, string?> SaveItems { get; }

        public abstract Task LoadItemsToObject();

        public abstract Task SaveItemsOfObject();

        public abstract Task OpenObjectMetaDataWindow();

        public abstract Task OpenMetaDataWindow();

        public ObservableCollection<Expanses> ExpansesColl { get; set; }
        public HierarchicalTreeDataGridSource<Expanses> ExpansesSource { get; set; }
    }
}
