using Microsoft.EntityFrameworkCore;
using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Image
{
	public class ImgSetRepository:Repository<Entities.ImgSet>, IImgSetRepository
    {
		private readonly PosclientContext _context;
        public ImgSetRepository(PosclientContext context) : base(context)
        {

            _context = context;
        }

        public void Update(Client_DBAccess.Entities.ImgSet imgSet)
        {
            _context.ImgSets.Update(imgSet);
        }
    }
}
