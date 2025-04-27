using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosplane_API_ViewModel.Attribute;

namespace Cosplane_API_ViewModel.Employee
{
    public class UpdateEmployeeViewModel
    {
        public string? AccId { get; set; }
		[Required(ErrorMessage = ("Vui lòng điền Họ và Tên"))]
		[MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
		[NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
		public string Fullname { get; set; }
        [Required(ErrorMessage = ("Vui lòng điền địa chỉ"))]
        [MaxLength(100, ErrorMessage = ("Vui lòng nhập ít hơn 100 ký tự"))]
        public string Address { get; set; }
        [Phone(ErrorMessage = ("Vui lòng điền đúng định dạng Số điện thoại"))]
        [Required(ErrorMessage = ("Vui lòng điền số điện thoại"))]
        [MaxLength(15, ErrorMessage = ("Vui lòng nhập ít hơn 15 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Phone {  get; set; }

        [Email(ErrorMessage = ("Vui lòng điền đúng định dạng Email"))]
        [Required(ErrorMessage = ("Vui lòng điền Email"))]
        [MaxLength(50, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
        public string Mail {  get; set; }
		[AllowNull]
        public DateTime CreatedDate { get; set; }

        public static UpdateEmployeeViewModel Create(EmployeeViewModel model)
        {
            if (model != null)
            {
                return new UpdateEmployeeViewModel
                {
                    Fullname = model.Fullname,
                    Address = model.Address,
                    Phone = model.Phone,
                    Mail = model.Mail,
                
                };
            }
            return new UpdateEmployeeViewModel();
        }
    }
}
