using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Brand
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Creator { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Account CreatorNavigation { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
