using Client_ViewModel.SubCategory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.SubCategoryServices
{
    public interface ISubCategoryService
    {
        Task<List<SubCategoryViewModel>> GetAllByCategoryAsync(string categoryId, string token);
    }
    public class SubCategoryService : ISubCategoryService
    {
        private readonly HttpClient _httpClient;
        public SubCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SubCategoryViewModel>> GetAllByCategoryAsync(string categoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/SubCategory/GetAllByCategory/" + categoryId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<SubCategoryViewModel>>(content);
                    return results;
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
