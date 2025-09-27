using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class TypesOfMetaDatum
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<ItemMetaDatum> ItemMetaData { get; set; } = new List<ItemMetaDatum>();

    public virtual ICollection<ObjectMetaDatum> ObjectMetaData { get; set; } = new List<ObjectMetaDatum>();
}
