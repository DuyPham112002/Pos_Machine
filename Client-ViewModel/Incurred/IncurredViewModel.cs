using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Incurred
{
	public class IncurredViewModel
	{
		public string? Id { get; set; }
		public string? ShiftId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public double Amount { get; set; }
		public bool IsActive { get; set; }
    
    }
}
