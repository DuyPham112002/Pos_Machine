using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Response;
using Client_ViewModel.Shift;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.ShiftService
{
	public interface IShiftService
	{
		Task<ResponseBase> CreateShiftAsync(CreateShiftViewModel shift, string AccId);
		Task<ResponseBase> EndShiftAsync(string shifId);
		Task<ResponseValue<ShiftViewModel>> GetShiftAsync(string accId);
		Task<ResponseValue<IEnumerable<ShiftViewModel>>> GetShiftbyDateRange(DateTime begin, DateTime end);
		Task<ResponseValue<IEnumerable<ShiftViewModel>>> GetAllAsync();
		Task<ResponseValue<ShiftForManagerViewModel>> GetShiftByIdAsync(string shifId);

	}
	public class ShiftService : IShiftService
	{
		private readonly IUnitOfWork _uow;
		private readonly IDapperContext _dapper;
		public ShiftService(IUnitOfWork uow, IDapperContext dapper)
		{
			_dapper = dapper;
			_uow = uow;
		}

		public async Task<ResponseValue<IEnumerable<ShiftViewModel>>> GetShiftbyDateRange(DateTime begin, DateTime end)
		{
			try
			{
				string query = $"select S.AccId,A.Username, S.BeginAmount, S.EndAmount, S.TimeStart,S.TimeEnd from [Shift] as S join Account as A on A.Id = S.AccId where CONVERT(Date,S.TimeStart) >= CONVERT(date, '{begin}') and Convert(Date,S.TimeEnd) <= CONVERT(date, '{end}') and S.IsActive = 'false'";
				var shifts = await GetDapperDateAsync<ShiftViewModel>(query);
				if (shifts != null)
				{
					return ResponseValue<IEnumerable<ShiftViewModel>>.Result(true, 200, shifts);
				}
				else
				{
					return ResponseValue<IEnumerable<ShiftViewModel>>.Result(false, 404, null);
				}
			}catch (Exception ex)
			{
				return ResponseValue<IEnumerable<ShiftViewModel>>.Result(false, 500, null);
			}
		
		}
		public async Task<ResponseBase> CreateShiftAsync(CreateShiftViewModel shift, string AccId)
		{
			try
			{
				var exsit = await _uow.Shift.GetFirstOrDefaultAsync(p => p.TimeStart.Date == DateTime.Now.Date && p.AccId == AccId && p.IsActive == true);
				if(exsit == null)
				{
                    Shift startShift = new Shift()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccId = AccId,
                        BeginAmount = Convert.ToDouble(shift.BeginAmount.Replace("VNĐ", "")
                                   .Replace(".", "")
                                   .Replace(",", "")
                                   .Trim()),
                        TimeStart = DateTime.Now,
                        IsActive = true
                    };
					if(startShift.BeginAmount >= 10000)
					{
                        await _uow.Shift.AddAsync(startShift);
                        await _uow.SaveAsync();
                        return ResponseBase.Result(true, 200);
					}
					else
					{
                        return ResponseBase.Result(false, 404, $"Số tiền bắt đầu ca tối thiểu 10.000 VNĐ");
                    }
                    
                }
                return ResponseBase.Result(false, 404, $"Không thể tạo ca mới khi chưa kết thúc ca hiện tại");
            }
			catch (Exception ex)
			{
				return ResponseBase.Result(false, 500, $"Lỗi không tạo được ca làm: {ex.Message}");
			}
		}

		public async Task<ResponseBase> EndShiftAsync(string shifId)
		{
			try
			{
				var endshift = await _uow.Shift.GetFirstOrDefaultAsync(q => q.Id == shifId, "Incurreds");
				var isExistOrderInShift = await _uow.Order.GetAllAsync(a => a.ShiftId == shifId && a.Status == 2);
				/*var shiftBefore = await _uow.Shift.GetFirstOrDefaultAsync(q => q.TimeEnd.Value.Date == DateTime.Now.Date && q.IsActive == false);*/
				if (endshift != null)
				{
					double Amounttmp = 0;
					foreach (var ord in isExistOrderInShift)
					{
                        Amounttmp += ord.Total;
                    }
					endshift.EndAmount = endshift.BeginAmount + endshift.Incurreds.Where(p => p.IsActive).Sum(a => a.Amount) + Amounttmp;
                    endshift.TimeEnd = DateTime.Now;
					endshift.IsActive = false;


					_uow.Shift.Update(endshift);
					await _uow.SaveAsync();
					return ResponseBase.Result(true, 200);
				}
				else return ResponseBase.Result(false, 400, "Lỗi không thể kết thúc ca");

			}
			catch (Exception ex)
			{
				return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
			}
		}

		public async Task<ResponseValue<ShiftViewModel>> GetShiftAsync(string accId)
		{

			try
			{
				Shift isExistShift = await _uow.Shift.GetFirstOrDefaultAsync(a => a.AccId == accId && a.IsActive && a.TimeStart.Date == DateTime.Now.Date && a.TimeEnd == null);
				if (isExistShift != null)
				{
                    ShiftViewModel shiftViewModel = new ShiftViewModel()
					{
						AccId = accId,
						Id = isExistShift.Id,
						TimeStart = isExistShift.TimeStart,
						
						BeginAmount = isExistShift.BeginAmount,
                        IsActive = isExistShift.IsActive,
					};
					return ResponseValue<ShiftViewModel>.Result(true, 200, shiftViewModel);
				}
				else return ResponseValue<ShiftViewModel>.Result(false, 404, null);
			}
			catch (Exception ex)
			{
				return ResponseValue<ShiftViewModel>.Result(false, 500, null);
			}
		}

        public async Task<ResponseValue<IEnumerable<ShiftViewModel>>> GetAllAsync()
        {
            try
            {
                string query = $"select S.AccId,S.Id ,A.Username, S.BeginAmount, S.EndAmount, S.TimeStart,S.TimeEnd, S.IsActive from [Shift] as S join Account as A on S.AccId = A.Id order by S.TimeStart Desc";
                var shifts = await GetDapperDateAsync<ShiftViewModel>(query);
                if (shifts != null)
                {
                    return ResponseValue<IEnumerable<ShiftViewModel>>.Result(true, 200, shifts);
                }
                else
                {
                    return ResponseValue<IEnumerable<ShiftViewModel>>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<ShiftViewModel>>.Result(false, 500, null);
            }

        }

        public async Task<ResponseValue<ShiftForManagerViewModel>> GetShiftByIdAsync(string shifId)
        {
            try
            {
                Shift isExistShift = await _uow.Shift.GetFirstOrDefaultAsync(a => a.Id == shifId, "Acc");
                if (isExistShift != null)
                {
                    ShiftForManagerViewModel shiftViewModel = new ShiftForManagerViewModel()
                    {
                        AccId = isExistShift.AccId,
                        Id = isExistShift.Id,
						Username = isExistShift.Acc.Username,
                        TimeStart = isExistShift.TimeStart,
                        TimeEnd = isExistShift.TimeEnd,
                        BeginAmount = isExistShift.BeginAmount,
						EndAmount = isExistShift.EndAmount,
						IsActive = isExistShift.IsActive,
                    };
                    return ResponseValue<ShiftForManagerViewModel>.Result(true, 200, shiftViewModel);
                }
                else return ResponseValue<ShiftForManagerViewModel>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<ShiftForManagerViewModel>.Result(false, 500, null);
            }

        }
        private async Task<IEnumerable<T>> GetDapperDateAsync<T>(string query)
        {
            using (var conection = _dapper.CreateConnection())
            {
                try
                {
                    var result = await conection.QueryAsync<T>(query);
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

