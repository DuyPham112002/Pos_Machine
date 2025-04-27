using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Role
{
    public class RoleRepository:Repository<Entities.Role>,IRoleRepository
    {
        private readonly PosmanagementContext _context;
        public RoleRepository(PosmanagementContext context):base(context) 
        {
            _context = context;
        }
    }
}
