using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Account
{
    public interface IAccountRepository:IRepository<Entities.Account>
    {
        void Update(Entities.Account account);
    }
}
