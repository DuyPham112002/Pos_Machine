using Client_ViewModel.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int IsRequiredAttribute { get; set; }
        public bool IsActive { get; set; }
        public string AttributeSetId { get; set; }
        public string ImageSetId { get; set; }
        public string? Images { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int Quantity { get; set; }

        public List<AttributeViewModel> Attributes { get; set; }
    }
}
