using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Response
{
    public class HttpResponse
    {
        public int StatusCode { get; set; }
        public string ReasonPhrases { get; set; }
    }
}
