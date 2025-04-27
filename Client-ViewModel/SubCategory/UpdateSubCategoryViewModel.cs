using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.SubCategory
{
    public class UpdateSubCategoryViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public string CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên loại sản phẩm")]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
