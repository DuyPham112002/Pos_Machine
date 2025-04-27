using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Shift
{
    public class ShiftForManagerViewModel
    {
        public string Id { get; set; }
        public string AccId { get; set; }
        public string Username { get; set; }
        public double BeginAmount { get; set; }
        public double? EndAmount { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public bool IsActive {  get; set; }
    }
}
