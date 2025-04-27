using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class SubscriptionMapping
{
    public string Id { get; set; } = null!;

    public string SubscriptionId { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public string Creator { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Account CreatorNavigation { get; set; } = null!;

    public virtual Subscription Subscription { get; set; } = null!;
}
