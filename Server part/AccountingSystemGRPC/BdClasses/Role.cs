using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PermissionsForRole> PermissionsForRoles { get; set; } = new List<PermissionsForRole>();

    public virtual ICollection<RolesOfUser> RolesOfUsers { get; set; } = new List<RolesOfUser>();
}
