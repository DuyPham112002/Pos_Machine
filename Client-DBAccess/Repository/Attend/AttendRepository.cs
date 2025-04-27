using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Attend
{
	public class AttendRepository : Repository<Client_DBAccess.Entities.Attend>, IAttendRepository
	{
		private readonly PosclientContext _context;

		public AttendRepository(PosclientContext db) : base(db)
		{
			_context = db;
		}
		public void Update(Client_DBAccess.Entities.Attend attend)
		{
			_context.Attends.Update(attend);
		}

	}
}
