using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.SubCategory
{
    public interface ISubCategoryRepository : IRepository<Entities.SubCategory>
    {
        void Update(Entities.SubCategory subCategory);
    }
}
