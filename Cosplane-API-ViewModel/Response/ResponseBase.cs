
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Response
{
    public class ResponseBase
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public HttpResponse HttpResponse { get; set; }

        public static ResponseBase Result(bool success, int status, string message = null)
        {
            return new ResponseBase
            {
                IsSuccess = success,
                Message = message,
                HttpResponse = new HttpResponse { StatusCode = status, ReasonPhrases = ReasonPhrases.GetReasonPhrase(status) }
            };
        }
    }
}
