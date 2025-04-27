namespace Cosplane_API_ViewModel.Device
{
    public class GetAllDeviceByBrandAPIViewModel
    {
        public string DeviceId { get; set; }
        public string DeviceFingerPrint { get; set; }
        public string CurrentAccount { get; set; }
        public bool IsDeviceActive { get; set; }
        public string BrandName { get; set; }
        public string BrandId { get; set; }
        public bool IsBrandActive { get; set; }
    }
}
