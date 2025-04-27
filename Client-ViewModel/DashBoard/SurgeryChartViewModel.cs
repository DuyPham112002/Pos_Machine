using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Dashboard
{
    public class SurgeryChartViewModel
    {
        public double sinceLastMonth { get; set; }
        public List<data> data { get; set; } = new List<data>();
    }
    public class data
    {
        public string x { get; set; }
        public double y { get; set; }
    }
}
