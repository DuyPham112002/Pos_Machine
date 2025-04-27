
using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Device;
using System.Diagnostics.CodeAnalysis;

namespace Cosplane_MVC_ViewModel.Device

{
    public partial class GetBrandUpdateDeviceMVCViewModel
    {
        [AllowNull]
        public List<GetAllBrandAPIViewModel>? GetAllBrandToUpdateModel { get; set; }
        public UpdateDeviceAPIViewModel UpdateDevice { get; set; }
    }
}



