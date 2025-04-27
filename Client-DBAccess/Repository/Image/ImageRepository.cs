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
	public class ImageRepository:Repository<Entities.Image>, IImageRepository
    {
		private readonly PosclientContext _context;
        public ImageRepository(PosclientContext context) : base(context)
        {

            _context = context;
        }

        public void Update(Client_DBAccess.Entities.Image image)
        {
            _context.Images.Update(image);
        }
    }
}
