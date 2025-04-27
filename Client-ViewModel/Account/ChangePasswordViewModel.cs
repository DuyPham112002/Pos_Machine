using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Account
{
    public class ChangePasswordViewModel
    {
        public string AccId { get; set; }

        [Required(ErrorMessage = "Vui lòng điền mật khẩu trước đó")]
        [MinLength(8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        [MaxLength(30, ErrorMessage = ("Vui lòng nhập ít hơn 30 ký tự"))]
                [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng điền mật khẩu")]
        [MinLength(8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        [MaxLength(30, ErrorMessage = ("Vui lòng nhập ít hơn 30 ký tự"))]
                [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng điền xác nhận mật khẩu")]
        [MinLength(8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        [MaxLength(30, ErrorMessage = ("Vui lòng nhập ít hơn 30 ký tự"))]
        public string ConfirmPassword { get; set; }
    }
}
