using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class GroupingProperty
{
    public int Id { get; set; }

    public List<string> Name { get; set; } = null!;

    public virtual ICollection<GroupingPropertiesForItem> GroupingPropertiesForItems { get; set; } = new List<GroupingPropertiesForItem>();
}
