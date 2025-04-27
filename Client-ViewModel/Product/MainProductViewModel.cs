using Client_ViewModel.Category;
using Client_ViewModel.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class MainProductViewModel
    {
        //public int AmountProduct { get; set; } = 0;
        //public int PageIndex { get; set; } = 0;
        //public string SearchName { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string SubcategoryId { get; set; } = "";
        public List<CategoryViewModel> Categories { get; set; }
        public List<SubCategoryViewModel> SubCategories { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}
