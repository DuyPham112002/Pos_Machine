using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Setting
{
    public class SettingViewModel
    {
        public string? Id { get; set; }
        public string? BrandId { get;set; }
        public string? BrandName { get; set; }   
        [Required(ErrorMessage = ("Vui lòng điền địa chỉ quán"))]
        [MaxLength(150, ErrorMessage = ("Vui lòng nhập ít hơn 150 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Addrress { get; set; } 
        [Required(ErrorMessage = ("Vui lòng điền hotline"))]
        [MaxLength(15, ErrorMessage = ("Vui lòng nhập ít hơn 15 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Hotline { get;set; } 
        [Required(ErrorMessage = ("Vui lòng điền password wifi"))]
        [MinLength(8, ErrorMessage = ("Vui lòng nhập ít nhất 8 ký tự"))]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Wifi { get;set; }
        public float? Size { get; set; }
        public string? Device { get; set; }  
    }
}
