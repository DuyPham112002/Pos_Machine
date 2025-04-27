using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.OrderDetail
{
    public class OrderDetailRepository : Repository<Client_DBAccess.Entities.OrderDetail>, IOrderDetailRepository
    {

        private readonly PosclientContext _context;

        public OrderDetailRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
