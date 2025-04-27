using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Attribute
{
    public interface IAttributeRepository : IRepository<Entities.Attribute>
    {
        void Update(Entities.Attribute attribute);
    }
}
