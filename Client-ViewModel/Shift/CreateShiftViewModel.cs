using Client_ViewModel.Incurred;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Shift
{
	public class CreateShiftViewModel
	{
		public string? Id { get; set; }
		public string? AccId { get; set; }
		public DateTime TimeStart { get; set; }
		public DateTime TimeEnd { get; set; }
		public string BeginAmount { get; set; }
		public double EndAmount { get; set; }
		public bool IsActive { get; set; }

	}


}
