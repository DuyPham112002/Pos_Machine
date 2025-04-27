using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Account
{
    public interface IAccountRepository : IRepository<Client_DBAccess.Entities.Account>
    {
        void Update(Client_DBAccess.Entities.Account account);
    }
}
