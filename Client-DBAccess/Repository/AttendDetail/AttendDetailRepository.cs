using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Attend;
using Client_DBAccess.Repository.AttendDetai;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.AttendDetail
{
	public class AttendDetailRepository : Repository<Client_DBAccess.Entities.AttendDetail>, IAttendDetailRepository
	{
		private readonly PosclientContext _context;

		public AttendDetailRepository(PosclientContext db) : base(db)
		{
			_context = db;
		}
		public void Update(Client_DBAccess.Entities.AttendDetail attendDetail)
		{
			_context.AttendDetails.Update(attendDetail);
		}
	}
}
