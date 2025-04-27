using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Employee
{
    public interface IEmployeeRepository : IRepository<Entities.Employee>
    {
        void Update(Entities.Employee employee);
    }
}
