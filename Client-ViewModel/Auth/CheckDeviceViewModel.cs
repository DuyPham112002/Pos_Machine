using Client_ViewModel.Fingerprint;

namespace Client_ViewModel.Auth
{
    public class CheckDeviceViewModel
    {
        public CheckLoginViewModel login { get; set; }
        public CheckFingerprintViewModel? fingerprint { get; set; }
        public GetBrowserInfoViewModel? browserInfo { get; set; }
    }
}
