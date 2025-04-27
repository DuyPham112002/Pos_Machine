using Cosplane_API_ViewModel.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Account
{
    public class CreateFullAccountViewModel
    {
        public CreateAccountViewModel Account { get; set; }
        public CreateEmployeeViewModel Employee { get; set; }
    }
}
