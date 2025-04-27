using Client_ViewModel.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.SubCategory
{
    public class MainSubcategoryViewModel
    {
        public CreateSubCategoryViewModel? CreateSubCategoryViewModel { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
        public List<SubCategoryViewModel>? SubCategories { get; set; }
    }
}
