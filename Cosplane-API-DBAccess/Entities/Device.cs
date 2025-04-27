using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Device
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string DeviceFingerPrint { get; set; } = null!;

    public bool IsActive { get; set; }

    public byte[]? CurrentAccount { get; set; }

    public virtual Brand Brand { get; set; } = null!;
}
