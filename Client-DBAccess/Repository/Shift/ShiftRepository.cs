using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using Client_DBAccess.Repository.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Shift
{
	public class ShiftRepository: Repository<Client_DBAccess.Entities.Shift>, IShiftRepository
	{
		private readonly PosclientContext _context;

		public ShiftRepository(PosclientContext db) : base(db)
		{
			_context = db;
		}
		public void Update(Client_DBAccess.Entities.Shift Shift)
		{
			_context.Shifts.Update(Shift);
		}
	}
}
