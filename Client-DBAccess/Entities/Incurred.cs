using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Incurred
{
    public string Id { get; set; } = null!;

    public string ShiftId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Amount { get; set; }

    public bool IsActive { get; set; }

    public virtual Shift Shift { get; set; } = null!;
}
