using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class PermissionsForRole
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermId { get; set; }

    public virtual Permission Perm { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
