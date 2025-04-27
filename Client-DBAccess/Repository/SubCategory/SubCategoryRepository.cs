using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.SubCategory
{
    public class SubCategoryRepository : Repository<Client_DBAccess.Entities.SubCategory>, ISubCategoryRepository
    {
        private readonly PosclientContext _context;

        public SubCategoryRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.SubCategory subCategory)
        {
            _context.SubCategories.Update(subCategory);
        }
    }
}
