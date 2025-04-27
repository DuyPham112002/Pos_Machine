using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Image
{
    public class ChangeImageViewModel
    {
        [Required]
        public string AccId { get; set; }
        [Required]
        public string ImageSetId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        [MaxLength(1)]
        [MinLength(1)]
        public List<IFormFile> Images { get; set; }
    }
}
