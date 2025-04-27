using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.AttendVIewModel;
using Client_ViewModel.Category;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_API_Services.AttendService
{
    public interface IAttendanceService
    {
        Task<ResponseBase> CreateAsync(AttendanceViewModel create, string creator);
        Task<ResponseValue<AttendanceViewModel>> GetAsync(string attendId);
        Task<ResponseValue<IEnumerable<AttendanceViewModel>>> GetAllAsync();
        Task<ResponseBase> DeleteAsync(string attendId);
    }
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public AttendanceService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(AttendanceViewModel create, string creator)
        {
            try
            {
                Attend newAttend = new Attend()
                {
                    Id = create.Id,
                    DateStart = create.DateStart,
                    DateEnd = create.DateEnd,
                    CreatedBy = creator,
                    CreatedDate = DateTime.Now,
                };
                if(newAttend.DateEnd <= DateTime.Today && newAttend.DateStart <= DateTime.Now)
                {
                    if(newAttend.DateStart <= newAttend.DateEnd)
                    {
                        try
                        {
                            await _uow.Attend.AddAsync(newAttend);
                            await _uow.SaveAsync();
                            return ResponseBase.Result(true, 200, "Tạo chấm công thành công");
                        }
                        catch (Exception ex)
                        {
                            return ResponseBase.Result(false, 404, $"Lỗi hệ thống: {ex.Message}");
                        }
                    }
                    else
                    {
                        return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Vui lòng nhập ngày bắt đầu tối đa bằng ngày kết thúc");
                    }
                 
                }
                else
                {
                    return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Vui lòng nhập ngày chấm công tối đa bằng ngày hiện tại");
                }
           


            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống:{ex.Message}");
            }

        }

        public async Task<ResponseBase> DeleteAsync(string attendId)
        {
            try
            {
                Attend attend = await _uow.Attend.GetFirstOrDefaultAsync(a => a.Id == attendId);
                if (attend != null)
                {
                    try
                    {
                        _uow.Attend.Remove(attend);
                        await _uow.SaveAsync();
                        return ResponseBase.Result(true, 200, $"Xóa thành công");
                    }
                    catch (Exception ex)
                    {
                        return ResponseBase.Result(false, 404, $"Lỗi không thể xóa được");
                    }

                }
                else return ResponseBase.Result(false, 404, $"Lỗi không tìm thấy bất kỳ dữ liệu chấm công nào");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống:{ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<AttendanceViewModel>>> GetAllAsync()
        {
            try
            {
                string query = $"select Att.Id,Att.CreatedBy, Att.DateStart, Att.DateEnd, Att.CreatedDate from Attend as Att join Account as A on Att.CreatedBy = A.Id order by Att.CreatedBy DESC";
                var attend = await GetDapperDateAsync<AttendanceViewModel>(query);
                if (attend != null)
                {

                    return ResponseValue<IEnumerable<AttendanceViewModel>>.Result(true, 200, attend);


                }
                return ResponseValue<IEnumerable<AttendanceViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<AttendanceViewModel>>.Result(false, 500, null);
            }

        }

        public async Task<ResponseValue<AttendanceViewModel>> GetAsync(string attendId)
        {
            try
            {
                string query = $"select Att.CreatedBy, Att.DateStart, Att.DateEnd, A.Creator from Attend as Att join Account as A on Att.CreatedBy = A.Id where Att.Id = '{attendId}' ";
                using (var conection = _dapper.CreateConnection())
                {
                    var attend = await conection.QueryFirstAsync<AttendanceViewModel>(query);
                    if (attend != null)
                    {
                        return ResponseValue<AttendanceViewModel>.Result(true, 200, attend);
                    }
                    return ResponseValue<AttendanceViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<AttendanceViewModel>.Result(false, 500, null);
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
