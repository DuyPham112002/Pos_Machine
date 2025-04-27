using Cosplane_API_ViewModel.Brand;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Cosplane_MVC_Service.BrandService
{
    public interface IBrandService
    {
        Task<List<GetAllBrandAPIViewModel>> GetAllAsync(string token);
        Task<bool> CreateAsync(CreateBrandAPIViewModel model, string token);
        Task<bool> UpdateAsync(UpdateBrandAPIViewModel model, string token);
        Task<GetAllBrandAPIViewModel> GetBrandAsync(string token, string brandId);
        Task<bool> DeleteAsync(string token, string brandId);
        Task<bool> ActivateAsync(string token, string brandId);

    }
    public class BrandService : IBrandService
    {
        //fields
        private readonly IConfiguration _config;
        private readonly string baseAddress;

        //constructor with params
        public BrandService(IConfiguration config)
        {
            _config = config;
            baseAddress = _config.GetSection("BaseAddress").Value;

        }

        //methods
        //function to create a brand
        public async Task<bool> CreateAsync(CreateBrandAPIViewModel model, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsJsonAsync("api/Brand/Create", model);
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
            HttpResponseMessage response = await client.GetAsync("api/brand");
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

        //function to get a brand by brandId
        public async Task<GetAllBrandAPIViewModel> GetBrandAsync(string token, string brandId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync($"api/brand/{brandId}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                GetAllBrandAPIViewModel result = JsonConvert.DeserializeObject<GetAllBrandAPIViewModel>(content);
                return result;
            }
            else
            {
                return null;
            }
        }

        //function to update brand
        public async Task<bool> UpdateAsync(UpdateBrandAPIViewModel model, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PutAsJsonAsync("api/Brand", model);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //function to delete a brand by brandId
        public async Task<bool> DeleteAsync(string token, string brandId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.DeleteAsync($"api/Brand/{brandId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //function to activate a brand
        public async Task<bool> ActivateAsync(string token, string brandId)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PutAsync($"api/Brand/activate/{brandId}", null);
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
