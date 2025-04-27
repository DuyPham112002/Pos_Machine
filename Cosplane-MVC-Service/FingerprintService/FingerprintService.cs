using Client_ViewModel.Fingerprint;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Cosplane_MVC_Service.FingerprintService
{
    public interface IFingerprintService
    {
        Task<string> GenerateDeviceFingerprint(GetBrowserInfoViewModel model);
    }
    public class FingerprintService : IFingerprintService
    {
        //fields
        private readonly IConfiguration _config;
        private readonly string baseAddress;
        //constructor with params
        public FingerprintService(IConfiguration config)
        {
            _config = config;
            baseAddress = _config.GetSection("BaseAddress").Value;
        }

        //methods
        //function to create fingerprint device
        public async Task<string> GenerateDeviceFingerprint(GetBrowserInfoViewModel model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/Fingerprint/Create", model);
            if (response.IsSuccessStatusCode)
            {
                string fingerprint = await response.Content.ReadAsStringAsync();
                return fingerprint;
            }
            else
            {
                return null;
            }
        }
    }
}
