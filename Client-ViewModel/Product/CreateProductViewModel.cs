using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.SubCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class CreateProductViewModel : IValidatableObject
    {
        public CreateProductAPIViewModel CreateProduct { get; set; } = new CreateProductAPIViewModel();
        public List<CategoryViewModel>? Categories { get; set; }
        public List<SubCategoryViewModel>? SubCategories { get; set; }
        [ValidateNever]
        public CreateAttributeViewModel CreateAttribute { get; set; } = new CreateAttributeViewModel();
        public ProductImage Image { get; set; } = new ProductImage();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var product = CreateProduct;
            if (CreateProduct != null && CreateProduct.IsRequiredAttribute != 1)
            {
                if (CreateAttribute.Attributes != null)
                {
                    for (int i = 0; i < CreateAttribute.Attributes.Count; i++)
                    {
                        if (!CreateAttribute.Attributes[i].IsDeleted)
                        {
                            var attribute = CreateAttribute.Attributes[i];
                            var context = new ValidationContext(attribute, validationContext, null);
                            var results = new List<ValidationResult>();
                            var isValid = Validator.TryValidateObject(attribute, context, results, validateAllProperties: true);
                            foreach (var validationResult in results)
                            {
                                var memberNames = validationResult.MemberNames.Select(m => $"CreateAttribute.Attributes[{i}].{m}");
                                yield return new ValidationResult(validationResult.ErrorMessage, memberNames);
                            }
                        }
                    }
                }
            }
        }
    }
}
