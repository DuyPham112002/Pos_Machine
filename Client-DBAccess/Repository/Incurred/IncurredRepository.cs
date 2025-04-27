using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Incurred
{
	public class IncurredRepository : Repository<Client_DBAccess.Entities.Incurred>, IIncurredRepository
	{
		private readonly PosclientContext _context;

		public IncurredRepository(PosclientContext db) : base(db)
		{
			_context = db;
		}
		public void Update(Client_DBAccess.Entities.Incurred Incurred)
		{
			_context.Incurreds.Update(Incurred);
		}
	}
}
