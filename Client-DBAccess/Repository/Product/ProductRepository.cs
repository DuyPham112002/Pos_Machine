using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Product
{
    public class ProductRepository : Repository<Client_DBAccess.Entities.Product>, IProductRepository
    {
        private readonly PosclientContext _context;

        public ProductRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Product product)
        {
            _context.Products.Update(product);
        }
    }
}
