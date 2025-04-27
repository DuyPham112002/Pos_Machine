using Client_DBAccess.Repository.Account;
using Client_DBAccess.Repository.Attend;
using Client_DBAccess.Repository.AttendDetai;
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
    public interface IUnitOfWork
    {
        IRoleRepository Role { get; }
        IAccountRepository Account { get; }
        IManagerRepository Manager { get; }
        IEmployeeRepository Employee { get; }
        ITokenRepository Token { get; }
        IImgSetRepository ImgSet { get; }
        IImageRepository Image { get; }
        ICategoryRepository Category { get; }
        ISubCategoryRepository SubCategory { get; }
        IProductRepository Product { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        IPaymentRepository Payment { get; }
        IShiftRepository Shift { get; }
        IIncurredRepository Incurred { get; }

        IAttendDetailRepository AttendDetail { get; }
        IAttendRepository Attend { get; }
        IAttributeRepository Attribute { get; }
        IAttributeSetRepository AttributeSet { get; }

        ISettingRepository Setting { get; }
        Task SaveAsync();
    }
}
