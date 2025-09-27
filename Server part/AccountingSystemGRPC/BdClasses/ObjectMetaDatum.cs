using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class ObjectMetaDatum
{
    public int Id { get; set; }

    public int DataTypeId { get; set; }

    public int ObjectId { get; set; }

    public string Name { get; set; } = null!;

    public byte[] Data { get; set; } = null!;

    public virtual TypesOfMetaDatum DataType { get; set; } = null!;

    public virtual Object Object { get; set; } = null!;
}
