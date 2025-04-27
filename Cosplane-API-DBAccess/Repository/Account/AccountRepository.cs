using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Account
{
    public class AccountRepository:Repository<Entities.Account>, IAccountRepository
    {
        private readonly PosmanagementContext _context;
        public AccountRepository(PosmanagementContext context):base(context) 
        {
            _context = context;            
        }

        public void Update(Entities.Account account)
        {
            _context.Accounts.Update(account);   
        }
    }
}
