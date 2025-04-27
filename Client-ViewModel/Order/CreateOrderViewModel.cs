using Client_ViewModel.Category;
using Client_ViewModel.OrderDetail;
using Client_ViewModel.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Order
{
    public class CreateOrderViewModel
    {
        public CreateOrderAPIViewModel Order { get; set; }
        public List<CreateOrderDetailViewModel> OrderDetails { get; set; } = new List<CreateOrderDetailViewModel>();
        public List<CategoryViewModel>? Categories { get; set; }
    }
}
