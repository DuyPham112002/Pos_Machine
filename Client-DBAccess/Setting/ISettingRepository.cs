using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Setting
{
    public interface ISettingRepository : IRepository<Entities.Setting>
    {
        void Update(Entities.Setting setting);
    }
}
