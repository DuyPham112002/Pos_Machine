using Cosplane_API_ViewModel.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Employee
{
    public class EmployeeViewModel
    {
        public string? AccId { get; set; }
        public string? Username { get; set; }
        public string? Id { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }  
    }
}
