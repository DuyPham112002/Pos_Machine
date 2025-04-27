using AutoMapper;
using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.Product;
using Client_ViewModel.SubCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_MVC.MapingFactory
{
    public class MapingFactory : Profile
    {
        public MapingFactory()
        {
            CreateMap<CategoryViewModel, UpdateCategoryViewModel>().ReverseMap();
            CreateMap<SubCategoryViewModel, UpdateSubCategoryViewModel>().ReverseMap();
            CreateMap<ProductViewModel, UpdateProductViewModel>().ReverseMap();
            CreateMap<AttributeViewModel, UpdateAttributeAPIViewModel>().ReverseMap();
        }
    }
}
