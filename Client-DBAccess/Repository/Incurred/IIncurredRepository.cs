using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Incurred
{
	public interface IIncurredRepository : IRepository<Entities.Incurred>
	{
		void Update(Entities.Incurred incurred);
	}
}
