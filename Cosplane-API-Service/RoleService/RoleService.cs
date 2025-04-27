using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosplane_API_Service.RoleService
{
    public interface IRoleService
    {
        Task<int> CreateAsync(CreateRoleViewModel info);
        Task<RoleViewModel> GetByName(string name);
    }
    public class RoleService: IRoleService
    {
        private readonly IUnitOfWork _uow;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow; 
        }

        /// <summary>
        ///  Get Role by role name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Role {id,name} else return null</returns>
        public async Task<RoleViewModel> GetByName(string name)
        {
            Role role = await _uow.Role.GetFirstOrDefaultAsync(x => x.Name == name);
            if (role != null)
            {
                return new RoleViewModel { Name = role.Name, Id = role.Id };
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        ///  Create role for cosplane employee  
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Return code: 400: duplicated account; 200: success; 500: exception</returns>
        public async Task<int> CreateAsync(CreateRoleViewModel info)
        {
            //check duplicated name
            Role roleDb = await _uow.Role.GetFirstOrDefaultAsync(q => q.Name == info.Name);
            if (roleDb == null)
            {
                try
                {
                    Role newRole = new Role()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = info.Name,
                        IsActive = true
                    };
                    await _uow.Role.AddAsync(newRole);  
                    await _uow.SaveAsync();
                    return 200;
                }
                catch (Exception ex) {
                    return 500;
                }
                
            }
            else
            {
                return 400;
            }
        }
    }
}
