using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;


namespace Client_API_Services.RoleService
{
    public interface IRoleService
    {
        Task<Role> GetByNameAsync(string name);
        Task<Role> GetById(string id);
    }
    public class RoleService : IRoleService 
    {
        
        private readonly IUnitOfWork _uow;
        
        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        

        public async Task<Role> GetById(string id)
        {
            return await _uow.Role.GetAsync(id);
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _uow.Role.GetFirstOrDefaultAsync(x => x.Name == name && x.IsActive);
        }
    }
}
