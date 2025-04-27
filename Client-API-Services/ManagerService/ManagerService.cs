using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Account;
using Client_ViewModel.Manager;
using Client_ViewModel.Response;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client_API_Services.ManagerService
{
    public interface IManagerService
    {
        Task<ResponseBase> CreateAsync(CreateManagerViewModel manager, string creator);
        Task<ResponseValue<ManagerViewModel>> GetByAccIdAsync(string accId);
        Task<ResponseValue<IEnumerable<ManagerViewModel>>> GetAllByAccIdAsync(string accId);
        Task<ResponseBase> UpdateProfileAsync(UpdateManagerViewModel manager, string accId);
        Task<Manager> CheckManagerExistAsync(string accId);
        Task<ResponseBase> ChangePasswordAsync(ChangePasswordViewModel model, string accId);
        Task<ResponseBase> CheckMailExistAsync(string email);
        Task<int> UpdateActiveAsync(string accId);
    }
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public ManagerService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }
        public async Task<ResponseBase> CreateAsync(CreateManagerViewModel manager, string creator)
        {
            try
            {
                if (manager != null)
                    manager.TrimProperties();
                Manager newManager = new Manager()
                {
                    AccId = manager.Account.Id,
                    Id = Guid.NewGuid().ToString(),
                    Fullname = manager.Fullname,
                    Address = manager.Address,
                    Email = manager.Email,
                    Phone = manager.Phone,
                    Gender = manager.Gender,
                    Bio = manager.Bio,
                    ImgSetId = manager.ImageSetId,
                    DateOfBirth = manager.DateOfBirth
                };
                await _uow.Manager.AddAsync(newManager);
                await _uow.SaveAsync();
                return ResponseBase.Result(true, 200);
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<IEnumerable<ManagerViewModel>>> GetAllByAccIdAsync(string accId)
        {
            try
            {
                string query = "select A.Id as [AccId], A.Username, M.Fullname, M.Address, M.Phone, M.Email, M.Gender, M.DateOfBirth, A.CreatedDate, A.Creator, M.Bio, A.IsActive from Manager as M with(NoLock) join Account as A with(NoLock) on M.AccId = A.Id where A.Id != @AccId";
                var managers = await GetDapperDateAsync<ManagerViewModel>(query, new { AccId = accId });
                if (managers != null)
                {
                    return ResponseValue<IEnumerable<ManagerViewModel>>.Result(true, 200, managers);
                }
                return ResponseValue<IEnumerable<ManagerViewModel>>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<IEnumerable<ManagerViewModel>>.Result(false, 500, null);
            }
        }

        public async Task<ResponseValue<ManagerViewModel>> GetByAccIdAsync(string accId)
        {
            try
            {
                Manager manager = await _uow.Manager.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
                if (manager != null)
                {
                    ManagerViewModel data = new ManagerViewModel
                    {
                        AccId = manager.Acc.Id,
                        Username = manager.Acc.Username,
                        Fullname = manager.Fullname,
                        Address = manager.Address,
                        Email = manager.Email,
                        Phone = manager.Phone,
                        IsActive = manager.Acc.IsActive,
                        CreatedDate = manager.Acc.CreatedDate,
                        DateOfBirth = manager.DateOfBirth != null ? Convert.ToDateTime(manager.DateOfBirth.ToString()) : DateTime.Now,
                        Bio = manager.Bio,
                        Gender = manager.Gender,
                        ImageSetId = manager.ImgSetId,
                        Creator = manager.Acc.Creator
                    };

                    data.Images = new List<string>();
                    List<Image> images = await _uow.Image.GetAllAsync(q => q.ImgSetId == data.ImageSetId && q.IsActive);
                    foreach (Image image in images)
                    {
                        string[] splited = image.ImageUrl.Split('/');
                        data.Images.Add(splited[1]);
                    }

                    return ResponseValue<ManagerViewModel>.Result(true, 200, data);
                }
                return ResponseValue<ManagerViewModel>.Result(false, 404, null);
            }
            catch (Exception ex)
            {
                return ResponseValue<ManagerViewModel>.Result(false, 500, null);
            }
        }


        public async Task<ResponseBase> UpdateProfileAsync(UpdateManagerViewModel manager, string accId)
        {

            try
            {
                var current = await _uow.Manager.GetFirstOrDefaultAsync(q => q.AccId == manager.AccId, "Acc");
                if (current != null)
                {
                    if (manager.Email != current.Email)
                        if ((await CheckMailExistAsync(manager.Email))?.IsSuccess == false)
                            return ResponseBase.Result(false, 400, "Email đã tồn tại trong hệ thống");
                    current.Fullname = manager.Fullname;
                    current.Gender = manager.Gender;
                    current.Phone = manager.Phone;
                    current.Email = manager.Email;
                    current.Address = manager.Address;
                    current.DateOfBirth = manager.DateOfBirth is DateTime dob ? DateOnly.FromDateTime(dob) : null;
                    current.Bio = manager.Bio;
                    current.Acc.LastestModifiedBy = accId;
                    current.Acc.LastestModifiedDate = DateTime.Now;
                    _uow.Manager.Update(current);
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
            Manager manager = await _uow.Manager.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
            if (manager != null)
            {
                try
                {
                    manager.Acc.IsActive = !manager.Acc.IsActive;
                    _uow.Account.Update(manager.Acc);
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
        public async Task<Manager> CheckManagerExistAsync(string accId)
        {
            Manager manager = await _uow.Manager.GetFirstOrDefaultAsync(a => a.AccId == accId, "Acc");
            if (manager != null)
            {
                return manager;
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

        private async Task<IEnumerable<T>> GetDapperDateAsync<T>(string query, object param)
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

        public async Task<ResponseBase> CheckMailExistAsync(string email)
        {
            return await _uow.Manager.GetFirstOrDefaultAsync(p => p.Email == email) != null ? ResponseBase.Result(false, 400, "Email đã tồn tại trong hệ thống") : ResponseBase.Result(true, 200);
        }
    }
}
