using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Token
{
    public class TokenRepository : Repository<Entities.Token>,ITokenRepository
    {
        private readonly PosmanagementContext _context;
        public TokenRepository(PosmanagementContext context):base(context) 
        {
            _context = context;
        }

        public void Update(Entities.Token token)
        {
            _context.Tokens.Update(token);
        }
    }
}
