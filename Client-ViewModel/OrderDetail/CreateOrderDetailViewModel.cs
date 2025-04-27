using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.OrderDetail
{
    public class CreateOrderDetailViewModel
    {
        public string? Id { get; set; }
        public string? AttributeId { get; set; }
        public string? Name { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public double SubTotal { get; set; }
    }
}
