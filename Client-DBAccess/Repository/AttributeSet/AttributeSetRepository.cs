using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.AttributeSet
{
    public class AttributeSetRepository : Repository<Entities.AttributeSet>, IAttributeSetRepository
    {
        private readonly PosclientContext _context;
        public AttributeSetRepository(PosclientContext context) : base(context)
        {

            _context = context;
        }

        public void Update(Client_DBAccess.Entities.AttributeSet attributeSet)
        {
            _context.AttributeSets.Update(attributeSet);
        }
    }
}
