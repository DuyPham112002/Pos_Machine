using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Token
{
    public string Id { get; set; } = null!;

    public string Value { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public string AccId { get; set; } = null!;

    public DateTime? ModifyDate { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
