using Client_ViewModel.Category;
using Client_ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class UpdateOrderViewModel
    {
        public UpdateOrderAPIViewModel Update { get; set; }
        public List<CreateOrderDetailViewModel> OrderDetails { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
    }
}
