
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Response
{
    public class ResponseValue<T> where T : class
    {
        public T? Value { get; set; }
        public bool IsSuccess { get; set; }
        public HttpResponse HttpResponse { get; set; }

        public static ResponseValue<T> Result(bool success, int status, T value = null)
        {
            return new ResponseValue<T>
            {
                IsSuccess = success,
                Value = value,
                HttpResponse = new HttpResponse { StatusCode = status, ReasonPhrases = ReasonPhrases.GetReasonPhrase(status) }
            };
        }
    }
}
