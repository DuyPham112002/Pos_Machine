using System;
using System.Collections.Generic;

namespace Cosplane_API_DBAccess.Entities;

public partial class Subscription
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<SubscriptionMapping> SubscriptionMappings { get; set; } = new List<SubscriptionMapping>();
}
