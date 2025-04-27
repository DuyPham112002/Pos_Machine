using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Employee
{
    public string AccId { get; set; } = null!;

    public string Id { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public virtual Account Acc { get; set; } = null!;
}
