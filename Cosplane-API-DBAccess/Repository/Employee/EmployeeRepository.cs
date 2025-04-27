using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_DBAccess.Repository.Employee
{
    public class EmployeeRepository:Repository<Entities.Employee>, IEmployeeRepository
    {
        private readonly PosmanagementContext _context;
        public EmployeeRepository(PosmanagementContext context):base(context) 
        {
            _context = context;   
        }

        public void Update(Entities.Employee entity)
        {
            _context.Employees.Update(entity);  
        }
    }
}
