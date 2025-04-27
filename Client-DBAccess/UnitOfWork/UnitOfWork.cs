using Client_DBAccess.Entities;
using Client_DBAccess.Repository.Account;
using Client_DBAccess.Repository.Attend;
using Client_DBAccess.Repository.AttendDetai;
using Client_DBAccess.Repository.AttendDetail;
using Client_DBAccess.Repository.Category;
using Client_DBAccess.Repository.Employee;
using Client_DBAccess.Repository.Image;
using Client_DBAccess.Repository.Incurred;
using Client_DBAccess.Repository.Manager;
using Client_DBAccess.Repository.Order;
using Client_DBAccess.Repository.OrderDetail;
using Client_DBAccess.Repository.Payment;
using Client_DBAccess.Repository.Product;
using Client_DBAccess.Repository.Role;
using Client_DBAccess.Repository.Shift;
using Client_DBAccess.Repository.SubCategory;
using Client_DBAccess.Repository.Token;
using Client_DBAccess.Repository.Attribute;
using Client_DBAccess.Repository.AttributeSet;
using Client_DBAccess.Setting;

namespace Client_DBAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PosclientContext _context;
        public IRoleRepository Role { get; private set; }

        public IAccountRepository Account { get; private set; }

        public IManagerRepository Manager { get; private set; }

        public IEmployeeRepository Employee { get; private set; }

        public ITokenRepository Token { get; private set; }
        public IImgSetRepository ImgSet { get; }
        public IImageRepository Image { get; }
        public IShiftRepository Shift { get; private set; }
        public IIncurredRepository Incurred { get; private set; }
        public ICategoryRepository Category { get; }
        public IAttendRepository Attend { get; private set; }
        public IAttendDetailRepository AttendDetail { get; private set; }
        public ISubCategoryRepository SubCategory { get; private set; }
        public IProductRepository Product { get; private set; }
        public IOrderRepository Order { get; }
        public IOrderDetailRepository OrderDetail { get; }
        public IPaymentRepository Payment { get; }
        public IAttributeRepository Attribute { get; }
        public IAttributeSetRepository AttributeSet { get; }
        public ISettingRepository Setting { get; }
        public UnitOfWork(PosclientContext context)
        {
            _context = context;
            Role = new RoleRepository(context);
            Account = new AccountRepository(context);
            Manager = new ManagerRepository(context);
            Employee = new EmployeeRepository(context);
            Token = new TokenRepository(context);
            ImgSet = new ImgSetRepository(context);
            Image = new ImageRepository(context);
            Category = new CategoryRepository(context);
            SubCategory = new SubCategoryRepository(context);
            Product = new ProductRepository(context);
            Order = new OrderRepository(context);
            OrderDetail = new OrderDetailRepository(context);
            Payment = new PaymentRepository(context);
            Attend = new AttendRepository(context);
            Shift = new ShiftRepository(context);
            Incurred = new IncurredRepository(context);
            Attribute = new AttributeRepository(context);
            AttributeSet = new AttributeSetRepository(context);
            AttendDetail = new AttendDetailRepository(context);
            Setting = new SettingRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Detach()
        {
            _context.ChangeTracker.Clear();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
