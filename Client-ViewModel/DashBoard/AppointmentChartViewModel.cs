using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Dashboard
{
    public class AppointmentChartViewModel 
    {
        public double sinceLastWeek { get; set; }
        public List<Param> data { get; set; } = new List<Param>();
    }
    public class Param
    {
        public string x { get; set; }
        public double y { get; set; }
    }
}
