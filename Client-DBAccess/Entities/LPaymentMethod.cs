using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class LPaymentMethod
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
