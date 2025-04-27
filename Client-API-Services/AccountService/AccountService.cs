using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Client_ViewModel.Account;
using Client_ViewModel.Response;

namespace Client_API_Services.AccountService
{
    public interface IAccountService
    {
        Task<ResponseBase> CreateAsync(CreateAccountViewModel accInfo, string creator, string brandId);
        Task<Account> CheckLoginAsync(string username, string password, string roleId);
    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDapperContext _dapper;
        public AccountService(IUnitOfWork uow, IDapperContext dapper)
        {
            _uow = uow;
            _dapper = dapper;
        }

        public async Task<Account> CheckLoginAsync(string username, string password, string roleId)
        {
            Account account = await _uow.Account.GetFirstOrDefaultAsync(q => q.Username == username && q.Password == password && q.IsActive && q.RoleId == roleId);
            return account;
        }

        public async Task<ResponseBase> CreateAsync(CreateAccountViewModel accInfo, string creator, string brandId)
        {
            try
            {
                if (accInfo != null)
                    accInfo.TrimProperties();
                Account existAccount = await _uow.Account.GetFirstOrDefaultAsync(q => q.Username == accInfo.Username && q.RoleId == accInfo.RoleId);
                if (existAccount == null)
                {
                    Account newAccount = new Account()
                    {
                        Id = accInfo.Id,
                        Username = accInfo.Username,
                        Password = accInfo.Password,
                        RoleId = accInfo.RoleId,
                        IsActive = true,
                        Creator = creator,
                        BrandId = brandId,
                        CreatedDate = DateTime.Now
                    };
                    await _uow.Account.AddAsync(newAccount);
                    await _uow.SaveAsync();
                    return ResponseBase.Result(true, 200);

                }
                else return ResponseBase.Result(false, 400, "Tên đăng nhập đã tồn tại");
            }
            catch (Exception ex)
            {
                return ResponseBase.Result(false, 500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
