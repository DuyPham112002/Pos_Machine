using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.SubCategory;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class ProductDetailViewModel : IValidatableObject
    {
        //For main view
        public ProductViewModel? ProductViewModel { get; set; }
        public List<AttributeViewModel>? Attributes { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
        public List<SubCategoryViewModel>? SubCategories { get; set; }
        public ProductImage? Image { get; set; }
        //For Update Action
        public UpdateProductViewModel? UpdateProductViewModel { get; set; }
        [ValidateNever]
        public UpdateAttributeViewModel UpdateAttributeViewModel { get; set; } = new UpdateAttributeViewModel();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var product = UpdateProductViewModel;
            if (UpdateProductViewModel != null && UpdateProductViewModel.IsRequiredAttribute != 1)
            {
                if (UpdateAttributeViewModel.Updates != null)
                {
                    for (int i = 0; i < UpdateAttributeViewModel.Updates.Count; i++)
                    {
                        if (!UpdateAttributeViewModel.Updates[i].IsDeleted)
                        {
                            var attribute = UpdateAttributeViewModel.Updates[i];
                            var context = new ValidationContext(attribute, validationContext, null);
                            var results = new List<ValidationResult>();
                            var isValid = Validator.TryValidateObject(attribute, context, results, validateAllProperties: true);
                            foreach (var validationResult in results)
                            {
                                var memberNames = validationResult.MemberNames.Select(m => $"UpdateAttributeViewModel.Updates[{i}].{m}");
                                yield return new ValidationResult(validationResult.ErrorMessage, memberNames);
                            }
                        }
                    }
                }
            }
        }
    }
}
