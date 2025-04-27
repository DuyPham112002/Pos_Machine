using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Setting
{
    public class SettingRepository : Repository<Entities.Setting>, ISettingRepository
    {
        private readonly PosclientContext _context;
        public SettingRepository(PosclientContext context) : base(context) 
        {
            _context = context;
        }
      
        public void Update(Entities.Setting account)
        {
            _context.Settings.Update(account);
        }
    }
}
