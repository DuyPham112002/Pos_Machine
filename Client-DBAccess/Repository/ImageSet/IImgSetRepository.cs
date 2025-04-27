using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Image
{
    public interface IImgSetRepository : IRepository<Entities.ImgSet>
    {
        void Update(Entities.ImgSet imgSet);
    }
}
