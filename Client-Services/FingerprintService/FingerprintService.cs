using Client_ViewModel.Fingerprint;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Client_Services.FingerprintService
{
    public interface IFingerprintService
    {
        Task<bool> CheckFingerprint(CheckFingerprintViewModel model);
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
        //function to create a brand
        public async Task<bool> CheckFingerprint(CheckFingerprintViewModel model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/Fingerprint/Check", model);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
