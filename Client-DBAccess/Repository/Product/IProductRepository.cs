using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Product
{
    public interface IProductRepository : IRepository<Entities.Product>
    {
        void Update(Entities.Product product);
    }
}
