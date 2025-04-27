using Client_ViewModel.Attribute;
using Client_ViewModel.Category;
using Client_ViewModel.Product;
using Client_ViewModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.AttributeService
{
    public interface IAttributeService
    {
        Task<ResponseBase> CreateAsync(CreateAttributeViewModel model, string token);
        Task<ResponseBase> UpdateAsync(UpdateAttributeViewModel model, string token);
        Task<List<AttributeViewModel>> GetByProductAsync(string productId, string token);
    }
    public class AttributeService : IAttributeService
    {
        private readonly HttpClient _httpClient;
        public AttributeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseBase> CreateAsync(CreateAttributeViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                model.Attributes = model.Attributes.Where(p => !p.IsDeleted).ToList();
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Attribute/Create", model);
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

        public async Task<ResponseBase> UpdateAsync(UpdateAttributeViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                model.Updates = model.Updates.Where(p => !p.IsDeleted).ToList();
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Attribute/Update", model);
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

        public async Task<List<AttributeViewModel>> GetByProductAsync(string productId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Attribute/GetAttByProduct/" + productId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<AttributeViewModel>>(content);
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
