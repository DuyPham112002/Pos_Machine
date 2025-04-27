using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.OrderActivityLog
{
    public class ActivityLogViewModel
    {
        public string Id { get; set; }
        public string LogActivated { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
