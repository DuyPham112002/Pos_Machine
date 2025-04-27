using Client_ViewModel.Attribute;
using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class UpdateProductViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string AttributeSetId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm")]
        public string CategoryId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public string SubCategoryId { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền tên sản phẩm"))]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Name { get; set; }
        #region logic price defaul or custom attribute  
        [DefaultValue(1)]
        [Required(ErrorMessage = ("Vui lòng chọn phân loại giá trị sản phẩm"))]
        [Range(1, 2, ErrorMessage = "Vui lòng chọn phân loại giá trị sản phẩm")]
        public int IsRequiredAttribute { get; set; } = 1;

        [Money("IsRequiredAttribute", ErrorMessage = "Giá tiền không hợp lệ")]
        [NoWhitespace(ErrorMessage = "Không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        [DefaultValue("0")]
        public string? Price { get; set; }
        #endregion
        [MaxLength(500, ErrorMessage = ("Vui lòng nhập ít hơn 500 ký tự"))]
        public string? Description { get; set; }
    }
}
