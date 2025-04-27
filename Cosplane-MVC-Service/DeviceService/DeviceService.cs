using Cosplane_API_ViewModel.Brand;
using Cosplane_API_ViewModel.Device;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Cosplane_MVC_Service.DeviceService
{
    public interface IDeviceService
    {
        Task<bool> CreateAsync(CreateDeviceAPIViewModel model, string token);
        Task<List<GetAllBrandAPIViewModel>> GetAllAsync(string token);
        Task<List<GetAllDeviceByBrandAPIViewModel>> GetListDeviceAsync(string token, string brandId);
        Task<bool> UpdateAsync(UpdateDeviceAPIViewModel model, string token);
        Task<bool> DeleteAsync(string token, string deviceId);
        Task<bool> ActivateAsync(string token, string deviceId);
        Task<GetDeviceByIdAPIViewModel> GetDeviceAsync(string token, string deviceId);
    }
    public class DeviceService : IDeviceService
    {
        //fields
        private readonly IConfiguration _config;
        private readonly string baseAddress;

        //constructor with params
        public DeviceService(IConfiguration config)
        {
            _config = config;
            baseAddress = _config.GetSection("BaseAddress").Value;
        }

        //methods
        //function to create a decive
        public async Task<bool> CreateAsync(CreateDeviceAPIViewModel model, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Device/Create", model);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //function to get the list of brands
        public async Task<List<GetAllBrandAPIViewModel>> GetAllAsync(string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync("api/device/brand");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<GetAllBrandAPIViewModel> result = JsonConvert.DeserializeObject<List<GetAllBrandAPIViewModel>>(content);
                return result;
            }
            else
            {
                return null;
            }
        }
        //function to get the list of device by brandId
        public async Task<List<GetAllDeviceByBrandAPIViewModel>> GetListDeviceAsync(string token, string brandId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync($"api/device/by/{brandId}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                List<GetAllDeviceByBrandAPIViewModel> result = JsonConvert.DeserializeObject<List<GetAllDeviceByBrandAPIViewModel>>(content);
                return result;
            }
            else
            {
                return null;
            }
        }

        //function to update device
        public async Task<bool> UpdateAsync(UpdateDeviceAPIViewModel model, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PutAsJsonAsync("api/Device", model);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //function to delete a device by deviceId
        public async Task<bool> DeleteAsync(string token, string deviceId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.DeleteAsync($"api/Device/{deviceId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //function to activate a deviceId
        public async Task<bool> ActivateAsync(string token, string deviceId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PutAsync($"api/Device/activate/{deviceId}", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //function to get a device by deviceId
        public async Task<GetDeviceByIdAPIViewModel> GetDeviceAsync(string token, string deviceId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync($"api/device/{deviceId}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                GetDeviceByIdAPIViewModel result = JsonConvert.DeserializeObject<GetDeviceByIdAPIViewModel>(content);
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
