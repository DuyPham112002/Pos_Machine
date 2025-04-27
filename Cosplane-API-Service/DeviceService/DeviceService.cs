using Cosplane_API_DBAccess.Dapper;
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Device;
using Dapper;

namespace Cosplane_API_Service.DeviceService
{
    public interface IDeviceService
    {
        Task<int> CreateDeviceAsync(CreateDeviceAPIViewModel info);
        Task<List<GetAllDeviceByBrandAPIViewModel>> GetDeviceByBrandId(string brandId);
        Task<bool> UpdateByDeviceIdAsync(UpdateDeviceAPIViewModel newDevice);
        Task<bool> DeleteByDeviceIdAsync(string deviceId);
        Task<bool> ActivateByDeviceIdAsync(string deviceId);
        Task<GetDeviceByIdAPIViewModel> GetDeviceById(string deviceId);
    }
    public class DeviceService : IDeviceService
    {
        //fields
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;

        //constructor with params
        public DeviceService(IUnitOfWork uow, IDapperContext dapper)
        {
            _dapper = dapper;
            _uow = uow;
        }

        //methods
        public async Task<int> CreateDeviceAsync(CreateDeviceAPIViewModel info)
        {
            //check duplicate name Device
          
                Device newDevice = new Device()
                {
                    DeviceFingerPrint = info.DeviceFingerPrint,
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    BrandId = info.BrandId,
                };
            await _uow.Device.AddAsync(newDevice);
            await _uow.SaveAsync();
            return 200;
        }

        //service to get all device by brandId using Dapper
        public async Task<List<GetAllDeviceByBrandAPIViewModel>> GetDeviceByBrandId(string brandId)
        {
            string query = @"select D.Id as DeviceId, D.DeviceFingerPrint as DeviceFingerPrint, D.IsActive as IsDeviceActive, 
                            D.CurrentAccount as CurrentAccount, B.Id as BrandId, B.Name as BrandName , B.IsActive as IsBrandActive
                            from Brand as B join Device as D on B.Id = D.BrandId where B.Id = @BrandId";

            using (var connection = _dapper.CreateConnection())
            {
                var result = await connection.QueryAsync<GetAllDeviceByBrandAPIViewModel>(query, new { BrandId = brandId });
                return result.ToList();
            }
        }
        //service to update a device by deviceId
        public async Task<bool> UpdateByDeviceIdAsync(UpdateDeviceAPIViewModel newDevice)
        {
            Device oldDevice = await _uow.Device.GetFirstOrDefaultAsync(q => q.Id == newDevice.DeviceId);

            if (oldDevice != null)
            {
                if (oldDevice.DeviceFingerPrint == newDevice.DeviceFingerPrint)
                {
                    // Update properties
                    oldDevice.BrandId = newDevice.BrandId;
                    oldDevice.DeviceFingerPrint = newDevice.DeviceFingerPrint;

                    try
                    {
                        _uow.Device.Update(oldDevice);
                        await _uow.SaveAsync();
                        return true;
                    }

                    catch (Exception ex)
                    {
                        // Log the exception
                        return false;
                    }
                }
                else
                {
                    Device isDuplicate = await _uow.Device.GetFirstOrDefaultAsync(q => q.DeviceFingerPrint == newDevice.DeviceFingerPrint);

                    if (isDuplicate == null)
                    {
                        // Update properties
                        oldDevice.BrandId = newDevice.BrandId;
                        oldDevice.DeviceFingerPrint = newDevice.DeviceFingerPrint;

                        try
                        {
                            _uow.Device.Update(oldDevice);
                            await _uow.SaveAsync();
                            return true;
                        }

                        catch (Exception ex)
                        {
                            // Log the exception
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else return false;
        }

        //service to delete a device by deviceId
        public async Task<bool> DeleteByDeviceIdAsync(string deviceId)
        {
            //check brand
            Device device = await _uow.Device.GetFirstOrDefaultAsync(q => q.Id == deviceId && q.IsActive);
            if (device != null)
            {
                //change status of IsActive
                device.IsActive = false;
                try
                {
                    //update status of account and save it
                    _uow.Device.Update(device);
                    await _uow.SaveAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }

        //service to activate a device is deleted by deviceId
        public async Task<bool> ActivateByDeviceIdAsync(string deviceId)
        {
            //check brand
            Device device = await _uow.Device.GetFirstOrDefaultAsync(q => q.Id == deviceId && !q.IsActive);
            if (device != null)
            {
                //change status of IsActive
                device.IsActive = !device.IsActive;
                try
                {
                    //update status of account and save it
                    _uow.Device.Update(device);
                    await _uow.SaveAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else return false;
        }

        //service to get a device by deviceId using Dapper
        public async Task<GetDeviceByIdAPIViewModel> GetDeviceById(string deviceId)
        {
            string query = @"select d.Id as DeviceId, d.DeviceFingerPrint as DeviceFingerprint, d.IsActive as IsDeviceActive,b.Id as BrandId, b.Name as BrandName, b.IsActive as IsBrandActive
                            from Device as d join Brand as b on d.BrandId = b.Id where d.Id = @DeviceId";

            using (var connection = _dapper.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetDeviceByIdAPIViewModel>(query, new { DeviceId = deviceId });
                return result;
            }
        }
    }
}

