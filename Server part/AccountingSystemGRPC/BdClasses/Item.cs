using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class Item
{
    public int Id { get; set; }

    public int? Objectid { get; set; }

    public int TypeUnitId { get; set; }

    public double CountOfUnits { get; set; }

    public double PricePerUnit { get; set; }

    public double ExcpectedCost { get; set; }

    public int ProducerId { get; set; }

    public string? Description { get; set; }

    public int TypeOfItemId { get; set; }

    public int NameId { get; set; }

    public double CountOfUsedUnits { get; set; }

    public virtual ICollection<GroupingPropertiesForItem> GroupingPropertiesForItems { get; set; } = new List<GroupingPropertiesForItem>();

    public virtual ICollection<ItemMetaDatum> ItemMetaData { get; set; } = new List<ItemMetaDatum>();

    public virtual NameItem Name { get; set; } = null!;

    public virtual Object? Object { get; set; }

    public virtual Producer Producer { get; set; } = null!;

    public virtual TypeOfItem TypeOfItem { get; set; } = null!;

    public virtual TypesOfUnit TypeUnit { get; set; } = null!;
}
