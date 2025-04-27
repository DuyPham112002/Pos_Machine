using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Order
{
    public string Id { get; set; } = null!;

    public string ShiftId { get; set; } = null!;

    public string Code { get; set; } = null!;

    public double Total { get; set; }

    public int Status { get; set; }

    public string? Note { get; set; }

    public string? ReasonCancel { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<OrderActivityLog> OrderActivityLogs { get; set; } = new List<OrderActivityLog>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual LStatus StatusNavigation { get; set; } = null!;
}
