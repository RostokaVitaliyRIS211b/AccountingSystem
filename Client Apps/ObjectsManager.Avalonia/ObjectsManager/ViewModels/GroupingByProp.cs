using Avalonia.Collections;
using Avalonia.Controls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    internal class GroupingByProp : DataGridGroupDescription
    {
        private int NumberOfProp { get; }
        public GroupingByProp(int numberOfProp)
        {
            NumberOfProp = numberOfProp;
        }

        public override object GroupKeyFromItem(object item, int level, CultureInfo culture)
        {
            var index = NumberOfProp - 1;
            if(item is ItemWrapper itemWrapper)
            {
                string result = "Остальное";
                if (itemWrapper.GroupingProperties.Count >= NumberOfProp)
                {
                    result = itemWrapper.GroupingProperties[index].Name;
                }
                return result;
            }
            return "Остальное";
        }
    }
}
