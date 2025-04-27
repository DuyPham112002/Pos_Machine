using System.ComponentModel.DataAnnotations;

namespace Cosplane_API_ViewModel.Account
{
    public class CreateAccountViewModel
    {
        public string? Id { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(200)]
        public string Username { get; set; }
        [Required]
        [MinLength(12, ErrorMessage = "Mật khẩu không được ngắn hơn 12 kí tự")]
        [MaxLength(400)]
        public string Password { get; set; }
        public string? RoleId { get; set; }
    }
}
