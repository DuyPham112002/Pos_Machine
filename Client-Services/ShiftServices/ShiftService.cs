using Client_ViewModel.Response;
using Client_ViewModel.Shift;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Services.ShiftServices
{
	public interface IShiftService
	{
		Task<ResponseBase> CreateAsync(CreateShiftViewModel shift, string token);
		Task<ResponseBase> EndAsync(string shiftId,string token);
		Task<ShiftViewModel> GetAsync(string token);
	}
	public class ShiftService : IShiftService
	{

		private readonly HttpClient _httpClient;
		public ShiftService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public async Task<ResponseBase> CreateAsync(CreateShiftViewModel shift, string token)
		{
			try
			{
				
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Shift/Start", shift);
				if(response.IsSuccessStatusCode)
				{
					return ResponseBase.Result(true, 200);
				}
				var converted = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<ResponseBase>(converted);

			}
			catch(Exception ex)
			{
				return null;
			}
		}

		public async Task<ResponseBase> EndAsync(string shiftId, string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.PutAsync("/api/Shift/End/" + shiftId, null);
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

		public async Task<ShiftViewModel> GetAsync(string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.GetAsync("/api/Shift");
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					var result = JsonConvert.DeserializeObject<ShiftViewModel>(content);
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
