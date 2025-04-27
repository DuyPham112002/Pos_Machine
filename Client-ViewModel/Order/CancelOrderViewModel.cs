using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class CancelOrderViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui lòng điền lý do hủy")]
        public string Resaon { get; set; }
    }
}
