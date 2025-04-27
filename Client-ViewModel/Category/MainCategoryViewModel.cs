using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Category
{
    public class MainCategoryViewModel
    {
        public CreateCategoryViewModel? CreateCategory { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
    }
}
