using System.ComponentModel.DataAnnotations;

namespace Cosplane_API_ViewModel.Attribute
{
    public class NoWhitespaceAttribute : ValidationAttribute
    {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is string stringValue && !stringValue.StartsWith(" ") && !stringValue.EndsWith(" "))
			{
				return ValidationResult.Success;
			}
			return new ValidationResult(this.ErrorMessage);
		}
	}
}
