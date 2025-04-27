using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Setting
{
    public string Id { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string Addrress { get; set; } = null!;

    public string Hotline { get; set; } = null!;

    public string Wifi { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }
}
