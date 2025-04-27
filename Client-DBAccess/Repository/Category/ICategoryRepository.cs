using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Category
{
    public interface ICategoryRepository : IRepository<Entities.Category>
    {
        void Update(Entities.Category category);
    }
}
