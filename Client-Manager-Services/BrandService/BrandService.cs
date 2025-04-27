using Cosplane_API_ViewModel.Brand;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Client_Manager_Services
{

    public interface IBrandService
    {
        Task<GetBrandIdByBrandNameAPIViewModel> GetBrandIdAsync(string brandName);
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

        public async Task<GetBrandIdByBrandNameAPIViewModel> GetBrandIdAsync(string brandName)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage response = await client.GetAsync($"/api/Brand/ByName/{brandName}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    GetBrandIdByBrandNameAPIViewModel result = JsonConvert.DeserializeObject<GetBrandIdByBrandNameAPIViewModel>(content);
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
