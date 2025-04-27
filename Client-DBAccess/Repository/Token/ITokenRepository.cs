using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Token
{
    public interface ITokenRepository : IRepository<Entities.Token>
    {
        void Update(Entities.Token token);
    }
}
