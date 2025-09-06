using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class TypesOfUnit
{
    public int Id { get; set; }

    public List<string> Name { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
