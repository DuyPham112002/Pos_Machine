using Client_ViewModel.Dashboard;
using Client_ViewModel.DashBoard;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.StaticalService
{
	public interface IStaticalService
	{
		Task<StaticalViewModel> GetAsync(string token);
		Task<DiseasesChartViewModel> GetDiseasesAsync(int type, string token);
        Task<LineChartViewModel> GetLinesAsync(int type, string token);
        Task<IEnumerable<OrderViewModel>> GetTopOrderAsync(int type, string token);
        Task<IEnumerable<ProductViewModel>> GetTopProductAsync(int type, string token);
    }
	public class StaticalService : IStaticalService
	{
		private readonly HttpClient _httpClient;
		public StaticalService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
        
        public async Task<StaticalViewModel> GetAsync(string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.GetAsync("/api/Statical");
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					var result = JsonConvert.DeserializeObject<StaticalViewModel>(content);
					return result;
				}
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

        public async Task<DiseasesChartViewModel> GetDiseasesAsync(int type, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Statical/GetDiseases/" + type);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<DiseasesChartViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<LineChartViewModel> GetLinesAsync(int type, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Statical/GetLines/" + type);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LineChartViewModel>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<OrderViewModel>> GetTopOrderAsync(int type, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Statical/GetTopOrders/" + type);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<IEnumerable<OrderViewModel>>(content);
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ProductViewModel>> GetTopProductAsync(int type, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Statical/GetTopProducts/" + type);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(content);
                    return result;
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
