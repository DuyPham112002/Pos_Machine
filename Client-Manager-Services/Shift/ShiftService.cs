using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.Shift;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Manager_Services.Shift
{
    public interface IShiftService
    {
        Task<List<ShiftForManagerViewModel>> GetAll(string token);
        Task<ShiftForManagerViewModel> GetAsync(string accId,string token);
    }
    public class ShiftService : IShiftService
    {
        private readonly HttpClient _httpClient;
        public ShiftService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ShiftForManagerViewModel>> GetAll(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Shift/GetAll");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<ShiftForManagerViewModel>>(content);
                    return results;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ShiftForManagerViewModel> GetAsync(string shiftId, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                HttpResponseMessage response = await _httpClient.GetAsync("/api/Shift/GetShiftbyId/" + shiftId);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<ShiftForManagerViewModel>(content);
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
