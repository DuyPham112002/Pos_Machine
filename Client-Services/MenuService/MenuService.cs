using Client_ViewModel.Menu;
using Client_ViewModel.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.MenuService
{
    public interface IMenuService
    {
        Task<List<MenuViewModel>> GetAllAsync(string token);
    }
    public class MenuService : IMenuService
    {
        private readonly HttpClient _httpClient;
        public MenuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MenuViewModel>> GetAllAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Menu/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<MenuViewModel>>(content);
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
