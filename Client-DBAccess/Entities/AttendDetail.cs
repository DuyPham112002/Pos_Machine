using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class AttendDetail
{
    public string Id { get; set; } = null!;

    public string AttendId { get; set; } = null!;

    public DateTime TimeStart { get; set; }

    public DateTime TimeEnd { get; set; }

    public double BeginBalance { get; set; }

    public double EndBalance { get; set; }

    public string AccId { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;

    public virtual Attend Attend { get; set; } = null!;
}
