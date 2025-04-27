namespace Cosplane_API_DBAccess.Entities;

public partial class Store
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string BrandId { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Address { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;


}
