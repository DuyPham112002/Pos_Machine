using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Category
{
    public class CategoryRepository : Repository<Client_DBAccess.Entities.Category>, ICategoryRepository
    {
        private readonly PosclientContext _context;

        public CategoryRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
