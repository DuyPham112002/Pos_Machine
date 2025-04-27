using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Role
{
	public class RoleRepository : Repository<Entities.Role>, IRoleRepository
	{
		private readonly PosclientContext _context;
		public RoleRepository(PosclientContext context) : base(context) // tương tự super trong java lấy contructor của lớp cha kế thửa sang lớp con
		{
			_context = context;
		}
		//methods
		public void Update(Entities.Role account)
		{
			_context.Roles.Update(account);
		}
	}
}
