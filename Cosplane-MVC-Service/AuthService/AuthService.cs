using Cosplane_API_ViewModel.Account;
using Cosplane_API_ViewModel.Token;
using Cosplane_MVC_ViewModel.Auth;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Cosplane_MVC_Service.AuthService
{
    public interface IAuthService
    {
        Task<TokenDecodedViewModel> CheckTokenAsync(string token);
        Task<LoginResultViewModel> AdminLoginAsync(BasicLoginViewModel model);
        Task<bool> Logout(string token);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly string baseAddress;

        public AuthService(IConfiguration config)
        {
            _config = config;
            baseAddress = _config.GetSection("BaseAddress").Value;
        }


        public async Task<TokenDecodedViewModel> CheckTokenAsync(string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsync("/api/admin/checktoken", null);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                TokenDecodedViewModel result = JsonConvert.DeserializeObject<TokenDecodedViewModel>(content);
                return result;
            }
            else
            {
                return null;
            }
        }
        public async Task<LoginResultViewModel> AdminLoginAsync(BasicLoginViewModel model)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/Login", model);
            if (response.IsSuccessStatusCode)
            {
                string token = await response.Content.ReadAsStringAsync();
                return new LoginResultViewModel()
                {
                    Status = true,
                    Message = token
                };
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    return new LoginResultViewModel()
                    {
                        Status = false,
                        Message = "Đã có lỗi xảy ra. Vui lòng thử lại"
                    };
                }
                else
                {
                    return new LoginResultViewModel()
                    {
                        Status = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                    };
                }
            }
        }

        public async Task<bool> Logout(string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/Admin/Logout", "");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else return false;
        }
    }
}
