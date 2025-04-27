using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Attend
{
    public string Id { get; set; } = null!;

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<AttendDetail> AttendDetails { get; set; } = new List<AttendDetail>();

    public virtual Account CreatedByNavigation { get; set; } = null!;
}
