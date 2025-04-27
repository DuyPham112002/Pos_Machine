using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Order
{
    public interface IOrderRepository : IRepository<Entities.Order>
    {
        void Update(Entities.Order order);
    }
}
