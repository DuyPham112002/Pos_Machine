using Client_DBAccess.Entities;
using Client_DBAccess.Repository.AttributeSet;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Attribute
{
    public class AttributeRepository : Repository<Entities.Attribute>, IAttributeRepository
    {
        private readonly PosclientContext _context;
        public AttributeRepository(PosclientContext context) : base(context)
        {

            _context = context;
        }

        public void Update(Client_DBAccess.Entities.Attribute attribute)
        {
            _context.Attributes.Update(attribute);
        }
    }
}
