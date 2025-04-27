using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Creator { get; set; } = null!;

    public DateTime? LastestModifiedDate { get; set; }

    public string? LastestModifiedBy { get; set; }

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
