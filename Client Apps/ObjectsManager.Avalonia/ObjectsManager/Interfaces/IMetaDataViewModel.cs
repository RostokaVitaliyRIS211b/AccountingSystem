using Avalonia.Controls;

using ObjectsManager.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IMetaDataViewModel
    {
        public Window? Win { get; set; }

        public Interaction<object?, string?> LoadFileInter { get; } 

        public Interaction<string, string?> SaveFileInter { get; }

        public TabItem? SelectedTabItem { get; set; }

        public abstract Task LoadFile();

        public abstract Task SaveFile();

        public IEnumerable<TabItem> SetTabItems(TabControl tabControl);
    }
}
