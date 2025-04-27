using Cosplane_API_DBAccess.Repository.Account;
using Cosplane_API_DBAccess.Repository.Brand;
using Cosplane_API_DBAccess.Repository.Device;
using Cosplane_API_DBAccess.Repository.Employee;
using Cosplane_API_DBAccess.Repository.Role;
using Cosplane_API_DBAccess.Repository.Token;

namespace Cosplane_API_DBAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
        IEmployeeRepository Employee { get; }
        IAccountRepository Account { get; }
        IRoleRepository Role { get; }
        ITokenRepository Token { get; }
        IBrandRepository Brand { get; }
        IDeviceRepository Device { get; }
    }
}
