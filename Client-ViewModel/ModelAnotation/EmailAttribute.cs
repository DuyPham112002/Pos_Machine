using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.ModelAnotation
{
    public class EmailAttribute : ValidationAttribute
    {
        private static bool EnableFullDomainLiterals { get; } =
            AppContext.TryGetSwitch("System.Net.AllowFullDomainLiterals", out bool enable) ? enable : false;

        public EmailAttribute()
        {

        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (!(value is string valueAsString))
            {
                return false;
            }

            if (!EnableFullDomainLiterals && (valueAsString.Contains('\r') || valueAsString.Contains('\n')))
            {
                return false;
            }

            int index = valueAsString.IndexOf('@');
            if (
               index <= 0 ||
               index == valueAsString.Length - 1 ||
               index != valueAsString.LastIndexOf('@')
               )
                return false;

            string domainPart = valueAsString.Substring(index + 1);
            if (!domainPart.Contains('.') || (domainPart.StartsWith('.') || domainPart.EndsWith('.')))
                return false;
            return true;
        }
    }
}
