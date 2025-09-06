using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PermissionsForRole> PermissionsForRoles { get; set; } = new List<PermissionsForRole>();
}
