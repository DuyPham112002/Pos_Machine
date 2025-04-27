using Client_ViewModel.Category;
using Client_ViewModel.Response;
using Client_ViewModel.SubCategory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.SubCategoryService
{
    public interface ISubCategoryService
    {
        Task<ResponseBase> CreateAsync(CreateSubCategoryViewModel model, string token);
        Task<ResponseBase> UpdateAsync(UpdateSubCategoryViewModel model, string token);
        Task<SubCategoryViewModel> GetSubCategoryAsync(string subCategoryId, string token);
        Task<List<SubCategoryViewModel>> GetAllByCategoryAsync(string categoryId, string token);
        Task<List<SubCategoryViewModel>> GetAllAsync(string token);
        Task<ResponseBase> UpdateActiveAsync(string categoryId, string token);
    }
    public class SubCategoryService : ISubCategoryService
    {
        private readonly HttpClient _httpClient;
        public SubCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ResponseBase> CreateAsync(CreateSubCategoryViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/SubCategory/Create", model);
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

        public async Task<List<SubCategoryViewModel>> GetAllAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/SubCategory/GetAll");
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

        public async Task<SubCategoryViewModel> GetSubCategoryAsync(string subCategoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/SubCategory/GetById/" + subCategoryId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SubCategoryViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseBase> UpdateActiveAsync(string categoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsync("/api/SubCategory/Active/" + categoryId, null);
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

        public async Task<ResponseBase> UpdateAsync(UpdateSubCategoryViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/SubCategory/update", model);
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
    }
}
