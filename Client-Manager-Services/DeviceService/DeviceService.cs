using Cosplane_API_ViewModel.Brand;
using Newtonsoft.Json;

namespace Client_Services.DeviceService
{
    public interface IDeviceService
    {

    }
    public class DeviceService : IDeviceService
    {
        //fields
        private readonly HttpClient _httpClient;
        //constructor with params
        public DeviceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        //methods
        public async Task<GetBrandIdByBrandNameAPIViewModel> GetBrandId(string brandName)
        {
            HttpResponseMessage respone = await _httpClient.GetAsync($"/api/Device/ByName/{brandName}");
            if (respone.IsSuccessStatusCode)
            {
                string content = await respone.Content.ReadAsStringAsync();
                GetBrandIdByBrandNameAPIViewModel result = JsonConvert.DeserializeObject<GetBrandIdByBrandNameAPIViewModel>(content);
                return result;
            }
            else return null;

        }
    }
}
