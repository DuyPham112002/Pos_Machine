using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Incurred;
using Client_ViewModel.Response;
using Client_ViewModel.Shift;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.IncurredService
{
	public interface IIcurredService
	{
		Task<ResponseBase> CreateIncurred(CreateIncurredViewModel incurred, string accId	);
		Task<ResponseValue<IEnumerable<IncurredViewModel>>> GetIncurredByShifIdAsync(string shifId );
	}
	public class IncurredService : IIcurredService
	{
		private readonly IUnitOfWork _uow;
		private readonly IDapperContext _dapper;
		public IncurredService (IUnitOfWork uwo, IDapperContext dapper)
		{
			_uow = uwo;
			_dapper = dapper;
		}
		public async Task<ResponseBase> CreateIncurred(CreateIncurredViewModel incurred,string accId)
		{
			try
			{
				var isExistShift = await _uow.Shift.GetFirstOrDefaultAsync(a=>a.AccId == accId && a.IsActive == true && a.TimeStart.Date == DateTime.Now.Date && a.TimeEnd == null);
				if (isExistShift != null)
				{
					Incurred newIncu = new Incurred()
					{
						Id = Guid.NewGuid().ToString(),
						ShiftId = isExistShift.Id,
						Amount = Convert.ToDouble(incurred.Amount.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim()),
						Description = incurred.Description,
						Title = incurred.Title,
						IsActive = true,
					};
					await _uow.Incurred.AddAsync(newIncu);
					await _uow.SaveAsync();
					return ResponseBase.Result(true, 200, "Tạo chi phí khác thành công");
				}
				else return ResponseBase.Result(false, 404, "Tạo chi phí khác thất bại");
				
				
			}
			catch (Exception ex)
			{
				return ResponseBase.Result(false, 500, "Lỗi hệ thống");
			}
		}

        public async Task<ResponseValue<IEnumerable<IncurredViewModel>>> GetIncurredByShifIdAsync(string shiftId)
        {
            try
            {
                string query = "select I.ShiftId, I.Amount,I.Description,I.Title,I.IsActive from Incurred as I join Shift as S on I.ShiftId = S.Id where S.Id = '" + shiftId+"'";
                var incurred = await GetDapperDateAsync<IncurredViewModel>(query);
                if (incurred != null)
                {
                    return ResponseValue<IEnumerable<IncurredViewModel>>.Result(true, 200, incurred);
                }
                return ResponseValue<IEnumerable<IncurredViewModel>>.Result(false, 500, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<IncurredViewModel>>.Result(false, 500, null);
            }
        }

    

        private async Task<IEnumerable<T>> GetDapperDateAsync<T>(string query, object? param = null)
        {
            using (var conection = _dapper.CreateConnection())
            {
                try
                {
                    var result = await conection.QueryAsync<T>(query, param);
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
