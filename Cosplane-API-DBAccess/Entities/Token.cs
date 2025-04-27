using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Token
{
    public string Id { get; set; } = null!;

    public string AccId { get; set; } = null!;

    public string Value { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
