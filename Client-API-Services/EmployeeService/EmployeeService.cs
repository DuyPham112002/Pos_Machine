using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Account;
using Client_ViewModel.Employee;
using Client_ViewModel.Response;
using Dapper;

namespace Client_API_Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<ResponseBase> CreateAsync(CreateEmployeeViewModel employee, string creator);
        Task<ResponseValue<EmployeeViewModel>> GetByAccIdAsync(string accId);
        Task<ResponseValue<IEnumerable<EmployeeViewModel>>> GetAllByAccIdAsync(string accId);
        Task<ResponseBase> UpdateProfileAsync(UpdateEmployeeViewModel employee, string accId);
        Task<ResponseBase> CheckMailExistAsync(string email);
        Task<Employee> CheckEmployeeExistAsync(string accId);
        Task<ResponseBase> ChangePasswordAsync(ChangePasswordViewModel model, string accId);
        Task<int> UpdateActiveAsync(string accId);
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

        public async Task<ResponseBase> CreateAsync(CreateEmployeeViewModel employee, string creator)
        {
            try
            {
                if (employee != null)
                    employee.TrimProperties();
                Employee newEmployee = new Employee()
                {
                    AccId = employee.Account.Id,
                    Id = Guid.NewGuid().ToString(),
                    Fullname = employee.Fullname,
                    Address = employee.Address,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Gender = employee.Gender,
                    Bio = employee.Bio,
                    ImgSetId = employee.ImageSetId,
                    DateOfBirth = employee.DateOfBirth
                };
                await _uow.Employee.AddAsync(newEmployee);
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<EmployeeViewModel>>> GetAllByAccIdAsync(string accId)
        {
            try
            {
                string query = "select A.Id as [AccId], A.Username, M.Fullname, M.Address, M.Phone, M.Email, M.Gender, M.DateOfBirth, A.CreatedDate, A.Creator, M.Bio, A.IsActive from employee as M with(NoLock) join Account as A with(NoLock) on M.AccId = A.Id";
                var Employees = await GetDapperDateAsync<EmployeeViewModel>(query);
                if (Employees != null)
                {
                    return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(true, 200, Employees);
                }
                return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<EmployeeViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<EmployeeViewModel>> GetByAccIdAsync(string accId)
        {
            try
            {
                Employee employee = await _uow.Employee.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
                if (employee != null)
                {
                    EmployeeViewModel data = new EmployeeViewModel
                    {
                        AccId = employee.Acc.Id,
                        Username = employee.Acc.Username,
                        Fullname = employee.Fullname,
                        Address = employee.Address,
                        Email = employee.Email,
                        Phone = employee.Phone,
                        IsActive = employee.Acc.IsActive,
                        CreatedDate = employee.Acc.CreatedDate,
                        DateOfBirth = employee.DateOfBirth != null ? Convert.ToDateTime(employee.DateOfBirth.ToString()) : DateTime.Now,
                        Bio = employee.Bio,
                        Gender = employee.Gender,
                        ImageSetId = employee.ImgSetId,
                        Creator = employee.Acc.Creator,
                       
                    };

                    data.Images = new List<string>();
                    List<Image> images = await _uow.Image.GetAllAsync(q => q.ImgSetId == data.ImageSetId && q.IsActive);
                    foreach (Image image in images)
                    {
                        string[] splited = image.ImageUrl.Split('/');
                        data.Images.Add(splited[1]);
                    }

                    return ResponseValue<EmployeeViewModel>.Result(true, 200, data);
                }
                return ResponseValue<EmployeeViewModel>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<EmployeeViewModel>.Result(false, 500, null);
            }
        }


        public async Task<ResponseBase> UpdateProfileAsync(UpdateEmployeeViewModel employee, string accId)
        {

            try
            {
                var current = await _uow.Employee.GetFirstOrDefaultAsync(q => q.AccId == employee.AccId, "Acc");
                if (current != null)
                {
                    if (employee.Email != current.Email)
                    {
                        if ((await CheckMailExistAsync(employee.Email))?.IsSuccess == false)
                            return ResponseBase.Result(false, 400, "Email đã tồn tại trong hệ thống");
                    }
                    current.Fullname = employee.Fullname;
                    current.Gender = employee.Gender;
                    current.Phone = employee.Phone;
                    current.Email = employee.Email;
                    current.Address = employee.Address;
                    current.DateOfBirth = employee.DateOfBirth is DateTime dob ? DateOnly.FromDateTime(dob) : null;
                    current.Bio = employee.Bio;
                    current.Acc.LastestModifiedBy = accId;
                    current.Acc.LastestModifiedDate = DateTime.Now;
                    _uow.Employee.Update(current);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, $"Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }

        }

        public async Task<int> UpdateActiveAsync(string accId)
        {
            Employee employee = await _uow.Employee.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
            if (employee != null)
            {
                try
                {
                    employee.Acc.IsActive = !employee.Acc.IsActive;
                    _uow.Account.Update(employee.Acc);
                    await _uow.SaveAsync();
                    return 200;
                }
                catch (Exception ex)
                {
                    return 500;
                }
            }
            else
            {
                return 404;
            }
        }
        public async Task<Employee> CheckEmployeeExistAsync(string accId)
        {
            Employee employee = await _uow.Employee.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
            if (employee != null)
            {
                return employee;
            }
            else return null;
        }

        public async Task<ResponseBase> ChangePasswordAsync(ChangePasswordViewModel model, string accId)
        {
            try
            {
                Account account = await _uow.Account.GetFirstOrDefaultAsync(q => q.Id == model.AccId && q.Password == model.OldPassword);
                if (account != null)
                {
                    if (model.NewPassword.ToUpper() != model.ConfirmPassword.ToUpper())
                    {
                        return ResponseBase.Result(false, 404, "Mật khẩu không trùng khớp. Vui lòng kiểm tra lại");
                    }
                    account.Password = model.NewPassword;
                    _uow.Account.Update(account);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);

                }
                else
                {
                    return ResponseBase.Result(false, 404, "Mật khẩu cũ không chính xác");
                }
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, ex.Message);
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

        public async Task<ResponseBase> CheckMailExistAsync(string email)
        {
            return await _uow.Employee.GetFirstOrDefaultAsync(p => p.Email == email) != null ? ResponseBase.Result(false, 400, "Email đã tồn tại trong hệ thống") : ResponseBase.Result(true, 200);
        }

    }
}
