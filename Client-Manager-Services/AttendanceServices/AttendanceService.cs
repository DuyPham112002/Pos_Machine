using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.AttendVIewModel;
using Client_ViewModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.AttendanceServices
{
	public interface IAttendanceService
	{
		Task<ResponseBase> CreateAsync(AttendanceViewModel model,string token);
		Task<List<AttendanceDetailViewModel>> GetAllByIdAsync(string attendId,string token);
		Task<ResponseBase> DeleteAsync(string attendId, string token);
		Task<List<AttendanceViewModel>> GetAllAsync(string token);
	}
	public class AttendanceService: IAttendanceService
	{
		private readonly HttpClient _httpClient;
		public AttendanceService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<ResponseBase> CreateAsync(AttendanceViewModel model, string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/Attendance/Create", model);
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

		public async Task<ResponseBase> DeleteAsync(string attendId, string token)	
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.DeleteAsync("/api/Attendance/DeleteById/"+ attendId);
                if (response.IsSuccessStatusCode)
                {
					return ResponseBase.Result(true, 200);
                }
				return ResponseBase.Result(false, 404, null);
            }
			catch(Exception ex)
			{
				return ResponseBase.Result(false,500, ex.Message);	
			}
		}

		public async Task<List<AttendanceDetailViewModel>> GetAllByIdAsync(string attendId,string token)
		{
			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Attendance/GetAllById/" + attendId);
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					var results = JsonConvert.DeserializeObject<List<AttendanceDetailViewModel>>(content);
					return results;
				}
				return null;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<List<AttendanceViewModel>> GetAllAsync(string token)
		{

			try
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				HttpResponseMessage response = await _httpClient.GetAsync("/api/Attendance/GetAll");
				if (response.IsSuccessStatusCode)
				{
					string content = await response.Content.ReadAsStringAsync();
					var results = JsonConvert.DeserializeObject<List<AttendanceViewModel>>(content);
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
