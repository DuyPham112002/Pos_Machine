using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class CreateOrderAPIViewModel
    {
        public string? Id { get; set; }
        [Required]
        public double Total { get; set; }
        public string? Note { get; set; }
    }
}
