using System;
using System.Collections.Generic;

namespace BdClasses;

public partial class User
{
    public int Id { get; set; }

    public List<string> Name { get; set; } = null!;

    public List<string> Password { get; set; } = null!;

    public virtual ICollection<RolesOfUser> RolesOfUsers { get; set; } = new List<RolesOfUser>();
}
