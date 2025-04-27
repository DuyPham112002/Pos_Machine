using Client_ViewModel.Category;
using Client_ViewModel.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.SubCategory
{
    public class DetailSubCategoryViewModel
    {
        public UpdateSubCategoryViewModel UpdateSubCategory { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
        public List<ProductViewModel>? Products { get; set; }
    }
}
