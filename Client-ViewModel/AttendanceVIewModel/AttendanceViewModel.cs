using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.AttendVIewModel
{
	public class AttendanceViewModel
	{
		public string? Id { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
		public string? CreateBy { get; set; } 
		public DateTime CreatedDate { get; set; }
	}
}
