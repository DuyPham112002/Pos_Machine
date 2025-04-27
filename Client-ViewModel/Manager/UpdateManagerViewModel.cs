using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Manager
{
    public class UpdateManagerViewModel
    {
        public string AccId { get; set; }
        public string Creator { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền tên quản lý"))]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền địa chỉ"))]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        public string Address { get; set; }
        [Phone(ErrorMessage = ("Vui lòng điền đúng định dạng"))]
        [Required(ErrorMessage = ("Vui lòng điền số điện thoại"))]
        [MaxLength(15, ErrorMessage = ("Vui lòng nhập ít hơn 15 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Phone { get; set; }
        [Email(ErrorMessage = ("Vui lòng điền đúng định dạng"))]
        [Required(ErrorMessage = ("Vui lòng điền email"))]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Email { get; set; }
        [Required(ErrorMessage = ("Vui lòng chọn giới tính"))]
        [Range(1, 2, ErrorMessage = "Vui lòng chọn giới tính")]
        public int Gender { get; set; }
        [AllowNull]
        public DateTime DateOfBirth { get; set; }
        [AllowNull]
        [DefaultValue("")]
        [MaxLength(500, ErrorMessage = ("Vui lòng nhập ít hơn 500 ký tự"))]
        public string? Bio { get; set; }
        public static UpdateManagerViewModel Create(ManagerViewModel model)
        {
            if (model != null)
            {
                return new UpdateManagerViewModel
                {
                    Fullname = model.Fullname,
                    Address = model.Address,
                    Phone = model.Phone,
                    Email = model.Email,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,
                    Bio = model.Bio
                };
            }
            return new UpdateManagerViewModel();
        }
    }
}
