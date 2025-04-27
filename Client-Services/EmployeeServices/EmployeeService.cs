using Client_ViewModel.Account;
using Client_ViewModel.Employee;
using Client_ViewModel.Image;
using Client_ViewModel.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.EmployeeServices
{
    public interface IEmployeeService
    {
        Task<ResponseBase> UpdateEmployeeAsync(UpdateEmployeeViewModel newProfile, string token);
        Task<ResponseBase> ChangePasswordAsync(ChangePasswordViewModel newPassword, string token);
        Task<EmployeeViewModel> GetEmployeeAsync(string accid, string token);
        Task<bool> UploadImage(CreateImageViewModel model, string token);
        Task<byte[]> GetDocument(string filename, string token);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;
        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseBase> ChangePasswordAsync(ChangePasswordViewModel newPassword, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Employee/ChangePassword", newPassword);
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

        public async Task<EmployeeViewModel> GetEmployeeAsync(string accid, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Employee/Get/" + accid);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseBase> UpdateEmployeeAsync(UpdateEmployeeViewModel newProfile, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/Employee/update", newProfile);
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

        public async Task<bool> UploadImage(CreateImageViewModel model, string token)
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

                HttpResponseMessage response = await _httpClient.PostAsync("/api/Employee/UploadImage", form);
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
