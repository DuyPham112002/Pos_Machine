
using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Device;
using System.Diagnostics.CodeAnalysis;

namespace Cosplane_MVC_ViewModel.Device

{
    public partial class GetBrandForDeviceMVCViewModel
    {
        [AllowNull]
        public List<GetAllBrandAPIViewModel>? GetAllBrandModel { get; set; }
        public CreateDeviceAPIViewModel CreateDeviceModel { get; set; }
    }
}
