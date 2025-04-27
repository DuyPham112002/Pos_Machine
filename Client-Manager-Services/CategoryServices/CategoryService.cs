using Client_ViewModel.Category;
using Client_ViewModel.Employee;
using Client_ViewModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<ResponseBase> CreateAsync(CreateCategoryViewModel model, string token);
        Task<ResponseBase> UpdateAsync(UpdateCategoryViewModel model, string token);
        Task<CategoryViewModel> GetCategoryAsync(string categoryId, string token);
        Task<List<CategoryViewModel>> GetAllCategoryAsync(string token);
        Task<ResponseBase> UpdateActiveAsync(string categoryId, string token);
    }
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseBase> CreateAsync(CreateCategoryViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Category/Create", model);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseBase.Result(true, 200);
                }
                var converted = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResponseBase>(converted);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, ex.Message);
            }
        }

        public async Task<ResponseBase> UpdateAsync(UpdateCategoryViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Category/update", model);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseBase.Result(true, 200);
                }
                var converted = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResponseBase>(converted);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, ex.Message);
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(string categoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsync("/api/Category/Active/" + categoryId, null);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseBase.Result(true, 200);
                }
                var converted = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResponseBase>(converted);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, ex.Message);
            }
        }

        public async Task<CategoryViewModel> GetCategoryAsync(string categoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Category/GetById/" + categoryId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CategoryViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
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
