using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Image
{
    public class CreateImageViewModel
    {
        [Required]
        public string ImgSetId { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public List<IFormFile> Images { get; set; }
    }
}
