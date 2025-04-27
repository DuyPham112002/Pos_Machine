using Client_ViewModel.Attribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.AttributeService
{
    public interface IAttributeService
    {
        Task<List<AttributeViewModel>> GetByProductAsync(string productId, string token);
    }
    public class AttributeService : IAttributeService
    {
        private readonly HttpClient _httpClient;
        public AttributeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
