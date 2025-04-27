namespace Cosplane_API_ViewModel.Device
{
    public class GetDeviceByIdAPIViewModel
    {
        public string DeviceId { get; set; }
        public string DeviceFingerprint { get; set; }
        public bool IsDeviceActive { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public bool IsBrandActive { get; set; }
    }
}
