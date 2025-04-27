using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class OrderForShiftViewModel
    {
        public string Id { get; set; }
        public string ShiftId { get; set; }
        public double Total { get; set; }
        public double Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Status { get; set; }
    }
}
