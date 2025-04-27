using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.AttendDetailViewModel;
using Client_ViewModel.AttendVIewModel;
using Client_ViewModel.Response;
using Client_ViewModel.Shift;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.AttendDetailService
{
    public interface IAttendanceDetailService
    {
        Task<ResponseBase> CreateAsync(AttendanceDetailViewModel model);
        Task<ResponseValue<IEnumerable<AttendanceDetailViewModel>>> GetAllByIdAsync(string attendId);
        Task<ResponseValue<IEnumerable<AttendDetail>>> DeleteDetailAsync(string attendId);
    }
    public class AttendanceDetailService : IAttendanceDetailService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public AttendanceDetailService(IUnitOfWork uow, IDapperContext dapper)
        {
            _dapper = dapper;
            _uow = uow;
        }

        public async Task<ResponseBase> CreateAsync(AttendanceDetailViewModel model)
        {
            try
            {
                AttendDetail newAttend = new AttendDetail()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccId = model.AccId,
                    AttendId = model.AttendId,
                    BeginBalance = model.BeginBalance,
                    EndBalance = model.EndBalance,
                    TimeStart = model.TimeStart,
                    TimeEnd = model.TimeEnd,
                };
                try
                {
                    await _uow.AttendDetail.AddAsync(newAttend);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200, $"Tạo chấm công thành công theo ngày từ {model.TimeStart} đến {model.TimeEnd}");
                }
                catch (Exception ex)
                {
                    return ResponseBase.Result(false, 400, $"Lỗi không thể tạo chấm công theo yêu cầu");
                }

            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống:{ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<AttendDetail>>> DeleteDetailAsync(string attendId)
        {
            try
            {
                string query = $"select Atd.Id ,Atd.AccId, Atd.AttendId,Atd.BeginBalance,Atd.EndBalance,Atd.TimeStart,Atd.TimeEnd from AttendDetail as Atd join Attend as A on Atd.AttendId = '{attendId}' and A.Id = '{attendId}'";
                var attendDetails = await GetDapperDateAsync<AttendDetail>(query);
                if (attendDetails != null)
                {
                    try
                    {
                        foreach (var attendanceDetail in attendDetails)
                        {
                            _uow.AttendDetail.Remove(attendanceDetail);
                            await _uow.SaveAsync();
                        }
                        return ResponseValue<IEnumerable<AttendDetail>>.Result(true, 200, null);
                    }
                    catch (Exception ex)
                    {
                        return ResponseValue<IEnumerable<AttendDetail>>.Result(false, 404, null);

                    }
                }
                else return ResponseValue<IEnumerable<AttendDetail>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<AttendDetail>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<IEnumerable<AttendanceDetailViewModel>>> GetAllByIdAsync(string AttendId)
        {
            try
            {
                string query = $"select A.Username,Atd.AccId, Atd.AttendId,Atd.BeginBalance,Atd.EndBalance,Atd.TimeStart,Atd.TimeEnd from AttendDetail as Atd join Account as A on Atd.AccId = A.Id where Atd.AttendId = '{AttendId}'";
                var attend = await GetDapperDateAsync<AttendanceDetailViewModel>(query);
                if (attend != null)
                {

                    return ResponseValue<IEnumerable<AttendanceDetailViewModel>>.Result(true, 200, attend);


                }
                return ResponseValue<IEnumerable<AttendanceDetailViewModel>>.Result(false, 404, [ ]);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<AttendanceDetailViewModel>>.Result(false, 500, null);
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
