using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Employee;
using Cosplane_API_ViewModel.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_MVC_Service.AdminService
{
    public interface IAdminService
    {
        Task<bool> CreateAsync(CreateFullAccountViewModel model, string token);
        Task<EmployeeViewModel> GetProfileAsync(string token);
        Task<bool> DeleteByAccountByIdAsync(string accId, string token);
        Task<ResponseBase> UpdateAsync(UpdateEmployeeViewModel model, string token);
        Task<List<EmployeeViewModel>> GetAllAsync(string token);
        Task<bool> ActivateAccountByIdAsync(string accId, string token);
        Task<EmployeeViewModel> GetEmployeeByIdAsync(string accId, string token);
        Task<bool> UpdateEmployeeAsync(UpdateEmployeeViewModel model, string token);
    }
    public class AdminService : IAdminService
    {

        private readonly IConfiguration _config;
        private readonly string baseAddress;
        public AdminService(IConfiguration config)
        {
            _config = config;
            baseAddress = _config.GetSection("BaseAddress").Value;
        }

        public async Task<bool> CreateAsync(CreateFullAccountViewModel model, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/Create", model);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteByAccountByIdAsync(string accId, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.PutAsync("/api/Admin/" + accId, null);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else

                    return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActivateAccountByIdAsync(string accId, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.PutAsync("/api/Admin/Activate/" + accId, null);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else

                    return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmployeeViewModel> GetProfileAsync(string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.GetAsync("/api/Admin/Profile");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    EmployeeViewModel result = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
                    return result;
                }
                else
                    return null;

            }
            catch
            {
                return null;
            }
        }


        public async Task<ResponseBase> UpdateAsync(UpdateEmployeeViewModel model, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.PutAsJsonAsync("/api/Admin", model);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseBase.Result(true, 200);
                }
                var converted = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResponseBase>(converted);

            }
            catch (Exception ex)
            {
                {
                    return ResponseBase.Result(false, 500, ex.Message);
                }
            }
        }

        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(string accId, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.GetAsync("/api/Admin/Detail/" + accId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    EmployeeViewModel result = JsonConvert.DeserializeObject<EmployeeViewModel>(content);
                    return result;
                }
                else
                    return null;

            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeViewModel model, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.PutAsJsonAsync("/api/Admin/Update", model);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else return false;

            }
            catch
            {
                return false;
            }
        }



        public async Task<List<EmployeeViewModel>> GetAllAsync(string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await client.GetAsync("/api/Admin/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(content);
                    return results;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }


    }
}
