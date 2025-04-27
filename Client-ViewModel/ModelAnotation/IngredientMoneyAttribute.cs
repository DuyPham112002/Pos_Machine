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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IngredientMoneyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Vui lòng điền giá tiền hợp lệ", new[] { validationContext.MemberName });
            }

            double number;
            bool isDouble = double.TryParse(value.ToString().Replace("VNĐ", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out number);

            if (!isDouble)
            {
                return new ValidationResult("Vui lòng điền đúng định dạng", new[] { validationContext.MemberName });
            }

            if (number <= 0)
            {
                return new ValidationResult("Vui lòng điền giá tiền hợp lệ", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
