using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.AttendDetailViewModel
{
	public class AttendanceDetailViewModel
	{
		public string? Id { get; set; }	
		public string AttendId { get; set; }
		public string AccId { get; set; }
		public string Username { get; set; }
		public DateTime TimeStart { get; set; }
		public DateTime TimeEnd { get; set; }
		public double BeginBalance { get; set; }
		public double EndBalance { get; set; }
	}
}
