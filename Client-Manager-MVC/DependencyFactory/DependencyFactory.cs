using Autofac;
using Autofac.Extensions.DependencyInjection;
using Client_Employee_Services.Employee;
using Client_Manager_Services.AttributeService;
using Client_Manager_Services;
using Client_Manager_Services.AttendanceServices;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.CategoryServices;
using Client_Manager_Services.ClientServices;
using Client_Manager_Services.ManagerService;
using Client_Manager_Services.MenuService;
using Client_Manager_Services.OrderService;
using Client_Manager_Services.ProductService;
using Client_Manager_Services.Shift;
using Client_Manager_Services.SubCategoryService;
using Client_Manager_Services.StaticalService;
using Client_Manager_Services.SettingService;

namespace Client_Manager_MVC.DependencyFactory
{
    public static class DependencyFactory
    {
        public static void AddDependencyInFactory(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            services.AddSingleton<ITypedClientConfig, TypedClientConfig>();

            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IAttributeService, AttributeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IOrderService, OrderService>(); 
			services.AddScoped<IMenuService, MenuService>();
			services.AddScoped<IStaticalService, StaticalService>();
            services.AddScoped<ISettingService, SettingService>();
        }
    }
}
