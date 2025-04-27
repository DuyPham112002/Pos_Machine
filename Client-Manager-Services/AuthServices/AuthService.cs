using Client_ViewModel.Auth;
using Client_ViewModel.Token;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Client_Manager_Services.AuthServices
{
    public interface IAuthService
    {
        Task<TokenResultViewModel> Login(CheckLoginViewModel model);
        Task<TokenDecodedViewModel> CheckTokenAsync(string token);
        Task<int> Logout(string token);
    }
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenDecodedViewModel> CheckTokenAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage respone = await _httpClient.PostAsJsonAsync("/api/Manager/CheckToken", new { });
                if (respone.IsSuccessStatusCode)
                {
                    string content = await respone.Content.ReadAsStringAsync();
                    TokenDecodedViewModel result = JsonConvert.DeserializeObject<TokenDecodedViewModel>(content);
                    return result;
                }
                else return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                Console.WriteLine(error);
                return null;
            }
        }

        public async Task<TokenResultViewModel> Login(CheckLoginViewModel model)
        {
            try
            {
                HttpResponseMessage respone = await _httpClient.PostAsJsonAsync("/api/Manager/Login", model);
                if (respone.IsSuccessStatusCode)
                {
                    string content = await respone.Content.ReadAsStringAsync();
                    TokenResultViewModel token = JsonConvert.DeserializeObject<TokenResultViewModel>(content);
                    return token;
                }
                else return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                Console.WriteLine(error);
                return null;
            }
        }

        public async Task<int> Logout(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage respone = await _httpClient.PutAsJsonAsync("/api/Manager/Logout", "");
                if (respone.IsSuccessStatusCode)
                    return 200;
                else return 404;
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                Console.WriteLine(error);
                return 500;
            }

        }
    }
}
