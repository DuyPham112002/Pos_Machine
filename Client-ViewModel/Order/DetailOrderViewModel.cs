using Client_ViewModel.OrderActivityLog;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class DetailOrderViewModel
    {
        public OrderViewModel? Order { get; set; }
        public IEnumerable<OrderDetailViewModel>? Orders { get; set; }
        public IEnumerable<ActivityLogViewModel>? Logs { get; set; }
        public CancelOrderViewModel Cancel { get; set; }
    }
}
