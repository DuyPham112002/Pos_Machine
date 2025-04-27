using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Attend
{
	public interface IAttendRepository : IRepository<Entities.Attend>
	{
		void Update(Entities.Attend attend);
	}
}
