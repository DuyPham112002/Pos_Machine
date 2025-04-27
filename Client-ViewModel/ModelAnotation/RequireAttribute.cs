using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.ModelAnotation
{
    public class RequireAttribute : ValidationAttribute
    {
        private readonly string _typePropertyName;
        private readonly int _typeValue;

        public RequireAttribute(string typePropertyName, int typeValue)
        {
            _typePropertyName = typePropertyName;
            _typeValue = typeValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var typeProperty = validationContext.ObjectType.GetProperty(_typePropertyName);
            if (typeProperty == null)
            {
                throw new ArgumentException($"Property with name {_typePropertyName} not found.");
            }
            var typeValue = (int?)typeProperty.GetValue(validationContext.ObjectInstance);
            if (typeValue == _typeValue)
            {
                // Validate the value
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
