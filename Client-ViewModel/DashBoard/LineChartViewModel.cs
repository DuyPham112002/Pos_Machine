using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.DashBoard
{
	public class LineChartViewModel
	{
		public double MaxValue { get; set; }
		public List<string> Labels { get; set; } = new List<string>();
		public List<series> Series { get; set; } = new List<series>();
	}

	public class series
	{
		public string name { get; set; } = string.Empty;
		public List<double> data { get; set; } = new List<double>();
	}
}
