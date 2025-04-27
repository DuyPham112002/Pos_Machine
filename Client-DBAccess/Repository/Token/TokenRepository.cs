using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Token
{
    public class TokenRepository : Repository<Entities.Token>, ITokenRepository
    {
        
        private readonly PosclientContext _context;
        
        public TokenRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Token token)
        {
            _context.Tokens.Update(token);
        }
    }
}
