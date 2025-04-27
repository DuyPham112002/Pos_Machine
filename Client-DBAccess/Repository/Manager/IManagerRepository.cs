using Client_DBAccess.Repository.Base;

namespace Client_DBAccess.Repository.Manager
{
    public interface IManagerRepository : IRepository<Entities.Manager>
    {
        void Update(Entities.Manager Manager);
    }
}
