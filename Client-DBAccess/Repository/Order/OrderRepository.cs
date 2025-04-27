using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Order
{
    public class OrderRepository : Repository<Client_DBAccess.Entities.Order>, IOrderRepository
    {

        private readonly PosclientContext _context;

        public OrderRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
