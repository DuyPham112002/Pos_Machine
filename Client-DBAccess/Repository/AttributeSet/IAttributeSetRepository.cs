using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.AttributeSet
{
    public interface IAttributeSetRepository : IRepository<Entities.AttributeSet>
    {
        void Update(Entities.AttributeSet attributeSet);
    }
}
