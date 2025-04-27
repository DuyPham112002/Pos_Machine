using Client_ViewModel.Account;
using Client_ViewModel.ModelAnotation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Client_ViewModel.ModelAnotation;
using System.ComponentModel.DataAnnotations;

namespace Client_ViewModel.Manager
{
    public class CreateManagerViewModel
    {
        public string? brandId { get; set; }
        public CreateAccountViewModel Account { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền tên quản lý"))]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Fullname { get; set; }
        [EmailAddress(ErrorMessage = ("Vui lòng điền đúng định dạng"))]
        [Required(ErrorMessage = ("Vui lòng điền email"))]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Email { get; set; }
        [Phone(ErrorMessage = ("Vui lòng điền đúng định dạng"))]
        [Required(ErrorMessage = ("Vui lòng điền số điện thoại"))]
        [MaxLength(15, ErrorMessage = ("Vui lòng nhập ít hơn 15 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Phone { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền địa chỉ"))]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        public string Address { get; set; }
        [Required(ErrorMessage = ("Vui lòng chọn giới tính"))]
        [Range(1, 2, ErrorMessage = "Vui lòng chọn giới tính")]
        public int Gender { get; set; }
        [AllowNull]
        [DefaultValue("")]
        [MaxLength(500, ErrorMessage = ("Vui lòng nhập ít hơn 500 ký tự"))]
        public string? Bio { get; set; }
        [AllowNull]
        [DefaultValue(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageSetId { get; set; }
        public void TrimProperties()
        {
            Fullname = Fullname?.Trim();
            Address = Address?.Trim();
            Email = Email?.Trim();
            Phone = Phone?.Trim();
            Bio = Bio?.Trim();
        }
    }
}
