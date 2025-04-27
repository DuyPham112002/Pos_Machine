using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class OrderDetail
{
    public string Id { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string? AttributeId { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public double Subtotal { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
