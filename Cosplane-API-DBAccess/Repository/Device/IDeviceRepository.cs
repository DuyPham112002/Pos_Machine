using Cosplane_API_DBAccess.Repository.Base;

namespace Cosplane_API_DBAccess.Repository.Device
{
    public interface IDeviceRepository : IRepository<Entities.Device>
    {
        void Update(Entities.Device device);
    }
}
