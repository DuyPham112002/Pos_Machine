using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class ImgSet
{
    public string Id { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
