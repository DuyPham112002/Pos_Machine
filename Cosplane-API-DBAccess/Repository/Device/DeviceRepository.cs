using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.Repository.Base;

namespace Cosplane_API_DBAccess.Repository.Device
{
    public class DeviceRepository : Repository<Entities.Device>, IDeviceRepository
    {
        //fields
        private readonly PosmanagementContext _context;

        //construtor with params
        public DeviceRepository(PosmanagementContext context) : base(context)
        {
            _context = context;
        }

        //methods
        public void Update(Entities.Device device)
        {
            _context.Devices.Update(device);
        }
    }
}
