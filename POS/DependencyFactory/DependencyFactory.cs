using Autofac;
using Autofac.Extensions.DependencyInjection;
using Client_POS_Services;
using Client_POS_Services.AuthServices;
using Client_POS_Services.ClientServices;
using Client_Services.AttributeService;
using Client_Services.CategoryServices;
using Client_Services.EmployeeServices;
using Client_Services.MenuService;
using Client_Services.FingerprintService;
using Client_Services.IncurredService;
using Client_Services.OrderDetailServices;
using Client_Services.OrderServices;
using Client_Services.PaymentService;
using Client_Services.ProductServices;
using Client_Services.ShiftServices;
using Client_Services.SubCategoryServices;
using Client_Services.SettingService;

namespace Client_POS.DependencyFactory
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
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAttributeService, AttributeService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IIncurredService, IncurredService>();
            services.AddScoped<IGenerateFingerprint, GenerateFingerprint>();
            services.AddScoped<IFingerprintService, FingerprintService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ISettingService, SettingService>();
        }
    }
}
