using Client_ViewModel.ModelAnotation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Payment
{
    public class CreatePaymentAPIViewModel
    {
        [Required]
        public string OrderId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        [Range(1, 2, ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        public int PaymentMethod { get; set; }
        [Required]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng điền số tiền khách hàng gửi")]
        [Money]
        [NoWhitespace(ErrorMessage = "không được chứa khoảng trắng trước và sau số tiền nhập")]
        public string Received { get; set; }
    }
}
