using Client_ViewModel.Incurred;
using Client_ViewModel.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Shift
{
    public class DetailShiftViewModel
    {
        public ShiftViewModel shift {  get; set; }
        public List<IncurredViewModel> incurred { get; set; }
        public List<OrderViewModel> order { get; set; }
    }
}
