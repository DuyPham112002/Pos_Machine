using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Store
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}
