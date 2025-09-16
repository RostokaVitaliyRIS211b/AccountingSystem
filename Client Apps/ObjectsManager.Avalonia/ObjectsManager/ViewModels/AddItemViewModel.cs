using Avalonia.Controls;

using ObjectsManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    public class AddItemViewModel:ViewModelBase, IAddItemViewModel
    {
        public Window? Win { get; set; }
    }
}
