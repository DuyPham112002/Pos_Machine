using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Payment
{
    public interface IPaymentRepository : IRepository<Entities.Payment>
    {
        void Update(Entities.Payment order);
    }
}
