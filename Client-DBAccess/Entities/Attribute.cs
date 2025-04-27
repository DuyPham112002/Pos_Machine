using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Attribute
{
    public string Id { get; set; } = null!;

    public string AttributeSetId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual AttributeSet AttributeSet { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
