using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
