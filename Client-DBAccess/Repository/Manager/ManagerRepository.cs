using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Manager
{
    public class ManagerRepository : Repository<Client_DBAccess.Entities.Manager>, IManagerRepository
    {
        
        private readonly PosclientContext _context;
        
        public ManagerRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Manager Manager)
        {
            _context.Managers.Update(Manager);
        }
    }
}
