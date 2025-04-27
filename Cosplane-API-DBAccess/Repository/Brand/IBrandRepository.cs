using Cosplane_API_DBAccess.Repository.Base;

namespace Cosplane_API_DBAccess.Repository.Brand
{
    public interface IBrandRepository : IRepository<Entities.Brand>
    {
        void Update(Entities.Brand brand);
    }
}
