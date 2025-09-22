using Avalonia.Controls;

using ObjectsManager.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.Interfaces
{
    public interface IEditGroupingPropsOfItemViewModel
    {
        public abstract Task AddGroupingProp();
        public abstract Task ApplyProps();
        public string? NameOfNewProp { get; set; }
        public string? FilterGProp { get; set; }
        public ObservableCollection<CheckGroupProp> ChGProps { get; set; }

        public ObservableCollection<CheckGroupProp> ChGPropsAll { get; set; }
        public Window? Win { get; set; }
    }
}
