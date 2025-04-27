using Client_ViewModel.Order;
using Client_ViewModel.Setting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.SettingService
{
    public interface ISettingService
    {
        Task<SettingViewModel> GetAsync(string brandId, string token);
    }
    public class SettingService : ISettingService
    {
        private readonly HttpClient _httpClient;
        public SettingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SettingViewModel> GetAsync(string brandId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Setting/Get/" + brandId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SettingViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
