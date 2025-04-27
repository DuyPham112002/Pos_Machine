using Client_ViewModel.Fingerprint;
using System.Text;

namespace Client_API_Services.Fingerprint
{
    public interface IGenerateFingerprint
    {
        Task<int> GenerateFingerprintDevice(GetBrowserInfoViewModel info);
        int SHA256ToNumber(string input);
    }
    public class GenerateFingerprint : IGenerateFingerprint
    {
        //fields
        private readonly string KEY = "×M<ã]4ó\u008eÔc¿;÷µ{÷Muöæ\u009arë^¥êâ¡×!¹§.µç\fïÝy\u009f¯87Ç±rW\u009c\u008a·\r\nï¿9Þ\u0099\"ïÎõ\u009f±û÷]ÂBæ\u009c\u008a·'v\u0089ej¾ýÛP\u0087»'\u009c\u008a·'I§\"­Å5ÓO8×M<ã]4ó";
        //methods
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
