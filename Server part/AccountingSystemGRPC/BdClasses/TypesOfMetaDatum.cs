using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class TypesOfMetaDatum
{
    public int Id { get; set; }

    public List<string> Name { get; set; } = null!;

    public virtual ICollection<ItemMetaDatum> ItemMetaData { get; set; } = new List<ItemMetaDatum>();
}
