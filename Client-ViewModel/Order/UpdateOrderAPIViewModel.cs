using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class UpdateOrderAPIViewModel
    {
        [Required]
        public string Id { get; set; }
        public string? Code { get; set; }
        public int? Quantity { get; set; }
        [Required]
        public double Total { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
    