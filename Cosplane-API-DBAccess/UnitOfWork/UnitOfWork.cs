using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Account;
using Cosplane_API_DBAccess.Repository.Brand;
using Cosplane_API_DBAccess.Repository.Device;
using Cosplane_API_DBAccess.Repository.Employee;
using Cosplane_API_DBAccess.Repository.Role;
using Cosplane_API_DBAccess.Repository.Token;

namespace Cosplane_API_DBAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PosmanagementContext _context;
        public IAccountRepository Account { get; private set; }
        public IRoleRepository Role { get; private set; }
        public ITokenRepository Token { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IDeviceRepository Device { get; private set; }


        public UnitOfWork(PosmanagementContext context)
        {
            _context = context;
            Account = new AccountRepository(_context);
            Role = new RoleRepository(_context);
            Token = new TokenRepository(_context);
            Employee = new EmployeeRepository(_context);
            Brand = new BrandRepository(_context);
            Device = new DeviceRepository(_context);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
