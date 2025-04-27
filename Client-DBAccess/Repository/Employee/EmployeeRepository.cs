using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Manager;
using Client_DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_DBAccess.Repository.Employee
{
    public class EmployeeRepository : Repository<Client_DBAccess.Entities.Employee>, IEmployeeRepository
    {
        
        private readonly PosclientContext _context;
        
        public EmployeeRepository(PosclientContext db) : base(db)
        {
            _context = db;
        }
        public void Update(Client_DBAccess.Entities.Employee employee)
        {
            _context.Employees.Update(employee);
        }
    }
}
