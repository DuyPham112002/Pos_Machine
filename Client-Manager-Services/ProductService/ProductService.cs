using Client_ViewModel.Category;
using Client_ViewModel.Image;
using Client_ViewModel.Product;
using Client_ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.ProductService
{
    public interface IProductService
    {
        Task<ResponseBase> CreateAsync(CreateProductAPIViewModel model, string token);
        Task<ResponseBase> UpdateAsync(UpdateProductViewModel model, string token);
        Task<ProductViewModel> GetProductAsync(string productId, string token);
        Task<List<ProductViewModel>> GetAllProductAsync(string token);
        Task<List<ProductViewModel>> GetAllBySubCategoryAsync(string subcategoryId, string token);
        Task<ResponseBase> UpdateActiveAsync(string productId, string token);
        Task<bool> UploadImage(ProductImage model, string token);
		Task<byte[]> GetDocument(string filename, string token);
	}
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ResponseBase> CreateAsync(CreateProductAPIViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Product/Create", model);
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

        public async Task<ResponseBase> UpdateActiveAsync(string productId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsync("/api/Product/Active/" + productId, null);
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

        public async Task<ResponseBase> UpdateAsync(UpdateProductViewModel model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Product/update", model);
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

        public async Task<bool> UploadImage(ProductImage model, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                using var form = new MultipartFormDataContent();
                foreach (IFormFile image in model.Images)
                {
                    byte[] data = await ConvertToByteArrayAsync(image);
                    var fileContent = new StreamContent(new MemoryStream(data));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                    form.Add(fileContent, "files", Path.GetFileName(image.FileName));
                    form.Add(new StringContent(model.ImgSetId), "imgSetId");
                }

                HttpResponseMessage response = await _httpClient.PostAsync("/api/Product/UploadImage", form);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
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

		private async Task<byte[]> ConvertToByteArrayAsync(IFormFile formFile)
        {
            try
            {
                if (formFile == null || formFile.Length == 0)
                    return null;

                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }
    }
}
