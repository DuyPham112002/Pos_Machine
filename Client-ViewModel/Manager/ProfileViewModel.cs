using Client_ViewModel.Account;
using Client_ViewModel.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Manager
{
    public class ProfileViewModel
    {
        public ManagerViewModel? Manager { get; set; }  
        public UpdateManagerViewModel? UpdateManager { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }
        public ChangeImageViewModel? ChangeImage { get; set; }
    }
}
