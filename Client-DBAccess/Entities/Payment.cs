using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Payment
{
    public string Id { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public int PaymentMethod { get; set; }

    public double Amount { get; set; }

    public double Received { get; set; }

    public double Changed { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual LPaymentMethod PaymentMethodNavigation { get; set; } = null!;
}
