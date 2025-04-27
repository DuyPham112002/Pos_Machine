using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Client_ViewModel.Auth
{
    public class CheckLoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng điền tên đăng nhập")]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        [StringLength(maximumLength: 400, MinimumLength = 5, ErrorMessage = "Vui lòng điền ít nhất 5 ký tự")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng điền mật khẩu")]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        [StringLength(maximumLength: 400, MinimumLength = 8, ErrorMessage = "Vui lòng điền ít nhất 8 ký tự")]
        public string Password { get; set; }
    }
}
