using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class ItemMetaDatum
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public int DataTypeId { get; set; }

    public int ItemId { get; set; }

    public virtual TypesOfMetaDatum DataType { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
