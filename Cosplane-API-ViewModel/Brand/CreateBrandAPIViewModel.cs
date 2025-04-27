using System.ComponentModel.DataAnnotations;

namespace Cosplane_API_ViewModel.Brand
{
    public class CreateBrandAPIViewModel
    {
        [Required]
        public string NameBrand { get; set; }
    }
}
