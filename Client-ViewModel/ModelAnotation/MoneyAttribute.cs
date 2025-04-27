using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.ModelAnotation
{
    public class MoneyAttribute : ValidationAttribute
    {
        private static bool EnableFullDomainLiterals { get; } =
            AppContext.TryGetSwitch("System.Net.AllowFullDomainLiterals", out bool enable) ? enable : false;
        public string DependentProperty { get; } = "";

        public MoneyAttribute()
        {
           
        }

        public MoneyAttribute(string dependentProperty)
        {
            DependentProperty = dependentProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int type = 2;
            PropertyInfo dependentPropertyInfo = validationContext.ObjectType.GetProperty(DependentProperty);
            if (dependentPropertyInfo != null)
            {
                var dependentValue = dependentPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                type = dependentValue != null && dependentValue is int intValue && intValue == 2 ? 2 : 1;
            }

            if (type == 1)
            {
                if (value == null || value.ToString() == string.Empty)
                {
                    return new ValidationResult(this.ErrorMessage);
                }

                double number;
                bool isDouble = double.TryParse(value.ToString().Replace("VNĐ", "")
                                       .Replace(".", "")
                                       .Replace(",", "")
                                       .Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out number);

                if (!isDouble)
                {
                    return new ValidationResult("Vui lòng điền đúng định dạng");
                }


                if (number <= 0)
                {
                    return new ValidationResult("Vui lòng điền giá tiền hợp lệ");
                }
            }

            return ValidationResult.Success;
        }
    }
}
