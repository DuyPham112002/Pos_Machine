using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.AttendDetai
{
	public interface IAttendDetailRepository : IRepository<Entities.AttendDetail>
	{
		void Update(Entities.AttendDetail attendDetail);
	}
}
