using Client_ViewModel.Dashboard;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.DashBoard
{
	public class StaticalViewModel
	{
		public IEnumerable<NumberSatical> Statisticals { get; set; }
		public AppointmentChartViewModel AppointmentChart { get; set; }
		public SurgeryChartViewModel SurgeryChart { get; set; }
		public DiseasesChartViewModel DiseasesChart { get; set; }
		public LineChartViewModel lineChart { get; set; }
		public IEnumerable<OrderViewModel> TopOrders { get; set; }
		public IEnumerable<ProductViewModel> TopProducts { get; set; }
	}
}
