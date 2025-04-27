using Client_ViewModel.Product;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.ProductServices
{
    public interface IProductService
    {
        Task<ProductViewModel> GetProductAsync(string productId, string token);
        Task<List<ProductViewModel>> GetAllBySubCategoryAsync(string subcategoryId, string token);
        Task<List<ProductViewModel>> GetAllProductAsync(string token);
        Task<byte[]> GetDocument(string filename, string token);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductViewModel> GetProductAsync(string productId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Product/GetById/" + productId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ProductViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProductViewModel>> GetAllProductAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Product/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
                    return results.OrderBy(o => o.Price).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ProductViewModel>> GetAllBySubCategoryAsync(string subcategoryId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Product/GetBySubcategory/" + subcategoryId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<byte[]> GetDocument(string filename, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Image/Document/" + filename);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    string base64 = content.Replace("-", "").ToString();
                    try
                    {
                        return Convert.FromBase64String(base64.Substring(1, base64.Length - 2));
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
