using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Account
{
    public class AccountRepository : Repository<Client_DBAccess.Entities.Account>, IAccountRepository
    {
        
        private readonly PosclientContext _context;
        
        public AccountRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        //methods
        public void Update(Client_DBAccess.Entities.Account account)
        {
            _context.Accounts.Update(account);
        }
    }
}
