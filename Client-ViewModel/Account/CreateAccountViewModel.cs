using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Account
{
    public class CreateAccountViewModel
    {
        public string? Id { get; set; }
        public string? RoleId { get; set; }

        [Required(ErrorMessage = "Vui lòng điền tên đăng nhập")]
        [MinLength(5, ErrorMessage = "Vui lòng điền ít nhất 5 ký tự")]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng điền mật khẩu")]
        [MinLength(8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        [MaxLength(30, ErrorMessage = ("Vui lòng nhập ít hơn 30 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng điền xác nhận mật khẩu")]
        [MinLength(8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        [MaxLength(30, ErrorMessage = ("Vui lòng nhập ít hơn 30 ký tự"))]
            [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string ConfirmPassword { get; set; }

        public void TrimProperties()
        {
            Username = Username?.Trim();
            Password = Password?.Trim();
            ConfirmPassword = ConfirmPassword?.Trim();
        }
    }
}
