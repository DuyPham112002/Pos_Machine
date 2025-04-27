using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Attribute
{
    public class UpdateAttributeAPIViewModel
    {
        public string? Id { get; set; }
        [IngredientMoney(ErrorMessage = "Giá tiền không hợp lệ")]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        [DefaultValue("0")]
        public string Price { get; set; }
        [Required(ErrorMessage = "Vui lòng điền tên phân loại")]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
