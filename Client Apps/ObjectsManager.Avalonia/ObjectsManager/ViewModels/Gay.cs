using Avalonia.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsManager.ViewModels
{
    internal class Gay : DataGridGroupDescription
    {
        private List<int> Users = [1,2,3,20];
        private List<int> Roles = [4,5,6,21,35];
        private List<int> Objects = [9,10,11,23,36,37];
        private List<int> Items = [13,14,15,19,24,25,26,27,28,29,30,31,32,33];
        public override object GroupKeyFromItem(object item, int level, CultureInfo culture)
        {
            if (item is CheckBoxPermission permission)
            {
                if (Users.Contains(permission.SpermPermission.Id))
                {
                    return "Пользователи";
                }
                else if (Roles.Contains(permission.SpermPermission.Id))
                {
                    return "Роли";
                }
                else if (Objects.Contains(permission.SpermPermission.Id))
                {
                    return "Объекты";
                }
                else if (Items.Contains(permission.SpermPermission.Id))
                {
                    return "Записи";
                }
            }
            return "Остальное";
        }
    }
}