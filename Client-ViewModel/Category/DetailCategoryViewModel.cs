using Client_ViewModel.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Category
{
    public class DetailCategoryViewModel
    {
        public UpdateCategoryViewModel? UpdateCategoryViewModel { get; set; }
        public List<SubCategoryViewModel>? SubCategories { get; set; }
        public UpdateSubCategoryViewModel? UpdateSubCategoryViewModel { get; set; }
    }
}
