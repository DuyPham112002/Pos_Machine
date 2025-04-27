using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class OrderActivityLog
{
    public int Id { get; set; }

    public string OrderId { get; set; } = null!;

    public string LogActivated { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
