using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Order;
using Client_ViewModel.Response;
using Client_ViewModel.Setting;
using Dapper;
using System.Text;

namespace Client_API_Services.SettingService
{
    public interface ISettingService
    {
        Task<ResponseBase> CreateAsync(SettingViewModel setting, string accId);
        Task<ResponseBase> UpdateAsync(SettingViewModel setting, string accId);
        Task<ResponseValue<SettingViewModel>> GetAsync(string brandId);
    }
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public SettingService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<ResponseBase> CreateAsync(SettingViewModel setting, string accId)
        {
            try
            {
                var current = await _uow.Setting.GetFirstOrDefaultAsync(p => p.BrandId == setting.BrandId);
                if(current == null)
                {
                    Setting newSetting = new Setting
                    {
                        Id = Guid.NewGuid().ToString(),
                        BrandId = setting.BrandId,
                        Addrress = setting.Addrress,
                        Hotline = setting.Hotline,
                        Wifi = setting.Wifi,
                        CreatedOn = DateTime.Now,   
                        CreatedBy = accId
                    };
                    await _uow.Setting.AddAsync(newSetting);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không thể tạo mới");
            }
            catch(Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ResponseValue<SettingViewModel>> GetAsync(string brandId)
        {
            try
            {
                string query = $"select * from Setting as S with(nolock) where S.BrandId = '{brandId}'";
                using (var conection = _dapper.CreateConnection())
                {
                    SettingViewModel order = await conection.QueryFirstAsync<SettingViewModel>(query);
                    if (order != null)
                    {
                        return ResponseValue<SettingViewModel>.Result(true, 200, order);
                    }
                    return ResponseValue<SettingViewModel>.Result(false, 404, null);
                }
            }
            catch (Exception ex)
            {
                return ResponseValue<SettingViewModel>.Result(false, 500, null);
            }
        }

        public async Task<ResponseBase> UpdateAsync(SettingViewModel setting, string accId)
        {
            try
            {
                var current = await _uow.Setting.GetFirstOrDefaultAsync(p => p.BrandId == setting.BrandId);
                if (current != null)
                {
                    current.Addrress = setting.Addrress;
                    current.Hotline = setting.Hotline;
                    current.Wifi = setting.Wifi;
                    current.ModifiedOn = DateTime.Now;
                    current.ModifiedBy = accId;
                    _uow.Setting.Update(current);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);
                }
                return ResponseBase.Result(false, 404, "Lỗi hệ thống: Không tìm thấy dữ liệu");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
