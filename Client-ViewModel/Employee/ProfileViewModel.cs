using Client_ViewModel.Account;
using Client_ViewModel.Image;
using Client_ViewModel.Manager;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Employee
{
    public class ProfileViewModel
    {
        public EmployeeViewModel? Employee { get; set; }
        public UpdateEmployeeViewModel? UpdateEmployee { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }
        public ChangeImageViewModel? ChangeImage { get; set; }
    }
}
