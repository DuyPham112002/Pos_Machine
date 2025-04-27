using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Token
{
    public interface ITokenRepository:IRepository<Entities.Token>
    {
        void Update(Entities.Token token);
    }
}
