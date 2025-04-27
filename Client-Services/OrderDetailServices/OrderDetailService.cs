using Client_ViewModel.Order;
using Client_ViewModel.OrderDetail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.OrderDetailServices
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetailViewModel>> GetAllAsync(string orederId, string token);
    }
    public class OrderDetailService : IOrderDetailService
    {
        private readonly HttpClient _httpClient;
        public OrderDetailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderDetailViewModel>> GetAllAsync(string orederId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/OrderDetail/GetAllByOrder/" + orederId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<OrderDetailViewModel>>(content);
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
