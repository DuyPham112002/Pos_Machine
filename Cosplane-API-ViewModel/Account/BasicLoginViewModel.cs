using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Account
{
    public class BasicLoginViewModel
    {
        [Required]
        [MinLength(8)]
        public string Username { get; set; }
        [Required]
        [MinLength(10)]
        public string Password { get; set; }
    }
}
