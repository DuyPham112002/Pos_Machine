using Client_ViewModel.Incurred;
using Client_ViewModel.Response;
using Client_ViewModel.Shift;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.IncurredService
{
	public interface IIncurredService
	{
		Task<ResponseBase> CreateIncurreAsync(CreateIncurredViewModel inc, string token);
		Task<List<IncurredViewModel>> GetIncurreByIdAsync(string shifId,string token);

    }
	public class IncurredService : IIncurredService
	{

		private readonly HttpClient _httpClient;
		public IncurredService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public async Task<ResponseBase> CreateIncurreAsync(CreateIncurredViewModel inc, string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Incurred/Create", inc);
				if (response.IsSuccessStatusCode)
				{
					return ResponseBase.Result(true, 200);
				}
				var converted = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ResponseBase>(converted);

			}
			catch (Exception ex)
			{
				return null;
			}
		}

        public async Task<List<IncurredViewModel>> GetIncurreByIdAsync(string shiftId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Incurred/" + shiftId );
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<IncurredViewModel>>(content);
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
