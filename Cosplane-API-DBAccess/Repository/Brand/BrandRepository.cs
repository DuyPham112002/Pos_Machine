using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;

namespace Cosplane_API_DBAccess.Repository.Brand
{
    public class BrandRepository : Repository<Entities.Brand>, IBrandRepository
    {
        //fields
        private readonly PosmanagementContext _context;

        //construtor with params
        public BrandRepository(PosmanagementContext context) : base(context)
        {
            _context = context;
        }

        //methods
        public void Update(Entities.Brand brand)
        {
            _context.Brands.Update(brand);
        }
    }
}
