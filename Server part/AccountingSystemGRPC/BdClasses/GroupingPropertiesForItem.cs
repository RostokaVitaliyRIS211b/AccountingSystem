using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class GroupingPropertiesForItem
{
    public int Id { get; set; }

    public int PropId { get; set; }

    public int ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual GroupingProperty Prop { get; set; } = null!;
}
