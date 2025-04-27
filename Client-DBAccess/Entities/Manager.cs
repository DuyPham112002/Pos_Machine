using System;
using System.Collections.Generic;

namespace Client_DBAccess.Entities;

public partial class Manager
{
    public string Id { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string AccId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Bio { get; set; }

    public string ImgSetId { get; set; } = null!;

    public int Gender { get; set; }

    public virtual Account Acc { get; set; } = null!;

    public virtual ImgSet ImgSet { get; set; } = null!;
}
