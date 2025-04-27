using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Shift
{
    public string Id { get; set; } = null!;

    public string AccId { get; set; } = null!;

    public DateTime TimeStart { get; set; }

    public DateTime? TimeEnd { get; set; }

    public double BeginAmount { get; set; }

    public double? EndAmount { get; set; }

    public bool IsActive { get; set; }

    public virtual Account Acc { get; set; } = null!;

    public virtual ICollection<Incurred> Incurreds { get; set; } = new List<Incurred>();
}
