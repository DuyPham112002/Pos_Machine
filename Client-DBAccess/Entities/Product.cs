using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string SubCategoryId { get; set; } = null!;

    public string? AttributeSetId { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string? Description { get; set; }

    public string ImgSetId { get; set; } = null!;

    public bool? IsRequiredAttribute { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsActive { get; set; }

    public virtual AttributeSet? AttributeSet { get; set; }

    public virtual ImgSet ImgSet { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual SubCategory SubCategory { get; set; } = null!;
}
