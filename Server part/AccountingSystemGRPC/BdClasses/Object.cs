using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class Object
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
