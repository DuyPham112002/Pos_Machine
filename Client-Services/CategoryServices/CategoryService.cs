using Client_ViewModel.Category;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<CategoryViewModel>> GetAllCategoryAsync(string token);
    }
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryViewModel>> GetAllCategoryAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Category/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<CategoryViewModel>>(content);
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
