using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? LastestModifiedBy { get; set; }

    public DateTime? LastestModifiedDate { get; set; }

    public string? BrandId { get; set; }

    public string Creator { get; set; } = null!;

    public virtual ICollection<AttendDetail> AttendDetails { get; set; } = new List<AttendDetail>();

    public virtual ICollection<Attend> Attends { get; set; } = new List<Attend>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Manager> Managers { get; set; } = new List<Manager>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
