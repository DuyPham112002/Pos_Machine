using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_ViewModel.Account;

namespace Cosplane_API_Service.AuthService
{
    public interface IAccountService
    {
        Task<int> CreateAsync(CreateAccountViewModel info, string creator);
        Task<Account> CheckLogin(BasicLoginViewModel info, string roleId);
        Task<bool> DeleteAccount(string accId);
        Task<bool> ActiveAccount(string accId);
    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        public AccountService(IUnitOfWork uow)
        {
            _uow = uow;
        }


		/// <summary>
		/// Check login with username and hashed password and role 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="roleId"></param>
		/// <returns>Account</returns>
		public async Task<Account> CheckLogin(BasicLoginViewModel info, string roleId)
        {
            Account account = await _uow.Account.GetFirstOrDefaultAsync(q => q.Username == info.Username && q.Password == info.Password && q.RoleId == roleId && q.IsActive);
            if (account != null)
            {
                return account;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create account. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Return code: 400: duplicated account; 200: success; 500: exception</returns>
        public async Task<int> CreateAsync(CreateAccountViewModel info, string creator)
        {
            //check duplicate username
            Account dbAccount = await _uow.Account.GetFirstOrDefaultAsync(q => q.Username == info.Username);
            if (dbAccount == null)
            {
                Account newAccount = new Account()
                {
                    Username = info.Username,
                    CreatedDate = DateTime.Now,
                    Creator = creator,
                    Id = info.Id,
                    IsActive = true,
                    LastestModifiedBy = creator,
                    LastestModifiedDate = DateTime.Now,
                    Password = info.Password,
                    RoleId = info.RoleId,

                };
                try
                {
                    await _uow.Account.AddAsync(newAccount);
                    await _uow.SaveAsync();
                    return 200;
                }
                catch
                {
                    return 500;
                }
            }
            else
            {
                return 400;
            }
        }

        //Delete by AccId
 

        public async Task<bool> DeleteAccount(string accId)
        {
            Account acc = await _uow.Account.GetFirstOrDefaultAsync(a => a.Id == accId, null);
            if (acc != null)
            {
                try
                {
                    acc.IsActive = false;
                    _uow.Account.Update(acc);
                    await _uow.SaveAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
		public async Task<bool> ActiveAccount(string accId)
		{
			Account acc = await _uow.Account.GetFirstOrDefaultAsync(a => a.Id == accId, null);
			if (acc != null)
			{
				try
				{
					acc.IsActive = true;
					_uow.Account.Update(acc);
					await _uow.SaveAsync();
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}
	}
}
