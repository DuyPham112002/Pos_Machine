using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Payment
{
    public class PaymentRepository : Repository<Client_DBAccess.Entities.Payment>, IPaymentRepository
    {

        private readonly PosclientContext _context;

        public PaymentRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Payment payment)
        {
            _context.Payments.Update(payment);
        }
    }
}
