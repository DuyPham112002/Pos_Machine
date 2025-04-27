using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Payment
{
    public class PaymentViewModel
    {
        public string Id { get; set; }
        public int PaymentMethod { get; set; }
        public double Amount { get; set; }
        public double Received { get; set; }
        public double Changed { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
