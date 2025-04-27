using Client_ViewModel.Fingerprint;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Client_Services.FingerprintService
{
    public interface IGenerateFingerprint
    {
        Task<string> GenerateDeviceFingerprint(GetBrowserInfoViewModel model);
    }
    public class GenerateFingerprint : IGenerateFingerprint
    {
        //fields
        private readonly IConfiguration _config;
        private readonly string baseUrl;
        //constructor with params
        public GenerateFingerprint(IConfiguration config)
        {
            _config = config;
            baseUrl = _config.GetSection("BaseURL").Value;
        }

        //methods
        //function to create fingerprint device
        public async Task<string> GenerateDeviceFingerprint(GetBrowserInfoViewModel model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
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
