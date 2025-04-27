
using Cosplane_API_DBAccess.Dapper;
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Employee;
using Cosplane_API_ViewModel.Response;
using Dapper;
namespace Cosplane_API_Service.EmployeeService
{
    public interface IEmployeeService
    {
        Task<ResponseBase> CreateAsync(CreateEmployeeViewModel info);
        Task<ResponseValue<EmployeeViewModel>> GetByIdAsync(string accId);
        Task<ResponseBase> UpdateAsync(UpdateEmployeeViewModel info, string accId);
        Task<ResponseValue<IEnumerable<EmployeeViewModel>>> GetAllAsync();
        Task<Employee> CheckEmployeeExist(string accId);
        Task<ResponseBase> CheckMailExist(string email);

    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public EmployeeService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }
        /// <summary>
        /// Create employee information. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Return code: 400: duplicated account; 200: success; 500: exception</returns>
        public async Task<ResponseBase> CreateAsync(CreateEmployeeViewModel info)
        {
            try
            {
                Employee newEmp = new Employee()
                {
                    AccId = info.AccId,
                    Address = info.Address,
                    Mail = info.Email,
                    Fullname = info.Fullname,
                    Id = Guid.NewGuid().ToString(),
                    Phone = info.Phone,
                };
                try
                {
					await _uow.Employee.AddAsync(newEmp);
					await _uow.SaveAsync();
					return ResponseBase.Result(true, 200,$"Tạo nhân viên thành công");
				}catch(Exception ex)
                {
                    return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không thể tạo thêm nhân viên");
                }
               

            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<EmployeeViewModel>> GetByIdAsync(string accId)
        {
            try
            {
                var getEmpById = await _uow.Employee.GetFirstOrDefaultAsync(a => a.AccId == accId);
                if (getEmpById != null)
                {
                    EmployeeViewModel data = new EmployeeViewModel()
                    {
                        Fullname = getEmpById.Fullname,
                        Address = getEmpById.Address,
                        Phone = getEmpById.Phone,
                        Mail = getEmpById.Mail
                    };
                    return ResponseValue<EmployeeViewModel>.Result(true, 200, data);
                }
                else return ResponseValue<EmployeeViewModel>.Result(false, 404,null);
            }
            catch (Exception ex)
            {
                return ResponseValue<EmployeeViewModel>.Result(false, 500,null);
            }
        }

        public async Task<ResponseBase> UpdateAsync(UpdateEmployeeViewModel info, string accId)
        {
            try
            {
                var oldInfo = await _uow.Employee.GetFirstOrDefaultAsync(q => q.AccId == accId, "Acc");
                if (oldInfo != null)
                {
                 /*   if (info.Mail != oldInfo.Mail)
                        if ((await CheckMailExist(info.Mail)).IsSuccess == false)
                            return ResponseBase.Result(false, 400, "Email đã tồn tại vui lòng chọn email khác");*/
                    oldInfo.Fullname = info.Fullname;
                    oldInfo.Address = info.Address;
                    oldInfo.Phone = info.Phone;
                    oldInfo.Mail = info.Mail;
                    oldInfo.Acc.CreatedDate = oldInfo.Acc.CreatedDate;
                    _uow.Employee.Update(oldInfo);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 20,"Cập nhật thông tin thành công");
                }
                return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu");

            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }

        }

        public async Task<ResponseValue<IEnumerable<EmployeeViewModel>>> GetAllAsync()
        {
            try
            {
                string query = "select A.Id as [AccId], A.Username, M.Fullname, M.Address, M.Phone,M.Mail, A.CreatedDate, A.IsActive  from employee as M with(NoLock) join Account as A  with(NoLock) on M.AccId = A.Id";
                var Employees = await GetDapperDateAsync<EmployeeViewModel>(query);
                if (Employees != null)
                {
                    return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(true, 200, Employees);
                }
                return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(false, 500, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(false, 500, null);
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

        public async Task<Employee> CheckEmployeeExist(string accId)
        {
            Employee employee = await _uow.Employee.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
            if (employee != null)
            {
                return employee;
            }
            else return null;
        }

        public async Task<ResponseBase> CheckMailExist(string email)
        {
            return await _uow.Employee.GetFirstOrDefaultAsync(p => p.Mail == email) != null ? ResponseBase.Result(false, 400, "Email đã tồn tại trong hệ thống") : ResponseBase.Result(true, 200);
        }
    }
}


