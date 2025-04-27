using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Product
{
    public class ProductImage
    {
        public string? ProductId { get; set; }
        public string? ImgSetId { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn hình ảnh sản phẩm")]
        [MinLength(1)]
        [MaxLength(1)]
        public List<IFormFile> Images { get; set; }
    }
}
