using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Image
{
    public interface IImageRepository : IRepository<Entities.Image>
    {
        void Update(Entities.Image image);
    }
}
