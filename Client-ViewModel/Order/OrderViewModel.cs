using Client_ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public double Total { get; set; }
        public string? ShiftId { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string? ReasonCancel { get; set; }
        public string Username { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
