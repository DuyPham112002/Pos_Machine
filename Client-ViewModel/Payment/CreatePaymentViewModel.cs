using Client_ViewModel.Order;
using Client_ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Payment
{
    public class CreatePaymentViewModel
    {
        public CreatePaymentAPIViewModel Create { get; set; }
        public OrderViewModel? Order { get; set; }
        public List<OrderDetailViewModel>? Orders { get; set; }
    }
}
