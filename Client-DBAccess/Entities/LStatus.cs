using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class LStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
