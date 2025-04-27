using Client_ViewModel.Brand;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.Payment;
using Client_ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class InvoiceOrderViewModel
    {
        public SettingViewModel Setting { get; set; }
        public OrderViewModel Order { get; set; }
        public IEnumerable<OrderDetailViewModel>? Orders { get; set; }
        public PaymentViewModel Payment { get; set; }
    }
}
