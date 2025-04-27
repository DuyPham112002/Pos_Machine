using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_ViewModel.Employee
{
    public class CreateEmployeeViewModel
    {
        public string? AccId { get; set; }
        [Required]
        [MinLength(8)]
        public string Fullname { get; set; }
        [MinLength(4)]
		[Phone(ErrorMessage = ("Vui lòng điền đúng định dạng Số điện thoại"))]
		[Required(ErrorMessage = ("Vui lòng điền số điện thoại"))]
		[MaxLength(15, ErrorMessage = ("Vui lòng nhập ít hơn 15 ký tự"))]
		[NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
		public string Phone { get; set; }
		[Email(ErrorMessage = ("Vui lòng điền đúng định dạng Email"))]
		[Required(ErrorMessage = ("Vui lòng điền Email"))]
		[MaxLength(200, ErrorMessage = ("Vui lòng nhập ít hơn 50 ký tự"))]
		[NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau dữ liệu được nhập")]
		public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(600)]
        public string Address { get; set; }

		public static CreateEmployeeViewModel Create(EmployeeViewModel model)
		{
			if (model != null)
			{
				return new CreateEmployeeViewModel
				{
					Fullname = model.Fullname,
					Address = model.Address,
					Phone = model.Phone,
					Email = model.Mail,

				};
			}
			return new CreateEmployeeViewModel();
		}
	}
}
