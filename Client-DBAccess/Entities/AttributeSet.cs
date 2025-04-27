using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class AttributeSet
{
    public string Id { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
