using Client_ViewModel.Fingerprint;
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using System.Text;

namespace Cosplane_API_Service.FingerprintService
{
    public interface ICheckFingerprintService
    {
        Task<bool> CheckFingerprint(string brandId, string deviceFingerprint);
        int SHA256ToNumber(string input);
        Task<int> GenerateFingerprintDevice(GetBrowserInfoViewModel info);
    }
    public class CheckFingerprintService : ICheckFingerprintService
    {
        //fields
        private readonly IUnitOfWork _uow;
        private readonly string KEY = "×M<ã]4ó\u008eÔc¿;÷µ{÷Muöæ\u009arë^¥êâ¡×!¹§.µç\fïÝy\u009f¯87Ç±rW\u009c\u008a·\r\nï¿9Þ\u0099\"ïÎõ\u009f±û÷]ÂBæ\u009c\u008a·'v\u0089ej¾ýÛP\u0087»'\u009c\u008a·'I§\"­Å5ÓO8×M<ã]4ó";


        //constructor with params
        public CheckFingerprintService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //methods
        public async Task<bool> CheckFingerprint(string brandId, string deviceFingerprint)
        {
            Device checkInfo = await _uow.Device.GetFirstOrDefaultAsync(q => q.DeviceFingerPrint == deviceFingerprint && q.BrandId == brandId && q.IsActive);
            if (checkInfo != null)
            {
                return true;
            }
            else return false;
        }

        //return a big integer from hash result
        public int SHA256ToNumber(string input)
        {
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                int hashNumber = BitConverter.ToInt32(hashBytes, 0);

                return Math.Abs(hashNumber);
            }
        }

        public async Task<int> GenerateFingerprintDevice(GetBrowserInfoViewModel info)
        {
            string data = info.OS + info.OSVersion + info.TimeZone + info.AvailableResolution +
                          info.Language + info.Browser + info.BrowserVersion +
                          info.Engine + info.Plugins + info.UserAgent +
                          info.AcceptLanguage + info.Scheme + KEY;
            int fingerprint = SHA256ToNumber(data);
            return fingerprint;
        }
    }
}
