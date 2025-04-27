using Client_Employee_Services.Employee;
using Client_Manager_Services.AttributeService;
using Client_Manager_Services.AttendanceServices;
using Client_Manager_Services.AuthServices;
using Client_Manager_Services.CategoryServices;
using Client_Manager_Services.ClientServices;
using Client_Manager_Services.ManagerService;
using Client_Manager_Services.MenuService;
using Client_Manager_Services.OrderService;
using Client_Manager_Services.ProductService;
using Client_Manager_Services.Shift;
using Client_Manager_Services.StaticalService;
using Client_Manager_Services.SubCategoryService;
using System.Net;
using Client_Manager_Services.SettingService;

namespace Client_Manager_MVC.HttpClientFactory
{
    public static class HttpClientFactory
    {
        public static void AddHttpClientInFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredHttpClient<IAuthService, AuthService>();
            services.AddConfiguredHttpClient<IManagerService, ManagerService>();
            services.AddConfiguredHttpClient<IEmployeeService, EmployeeService>();
            services.AddConfiguredHttpClient<ICategoryService, CategoryService>();
            services.AddConfiguredHttpClient<ISubCategoryService, SubCategoryService>();
            services.AddConfiguredHttpClient<IProductService, ProductService>();
            services.AddConfiguredHttpClient<IAttributeService, AttributeService>();
            services.AddConfiguredHttpClient<IOrderService, OrderService>();
            services.AddConfiguredHttpClient<IMenuService, MenuService>();
            services.AddConfiguredHttpClient<IAttendanceService, AttendanceService>();
            services.AddConfiguredHttpClient<IShiftService, ShiftService>();
			services.AddConfiguredHttpClient<IStaticalService, StaticalService>();
            services.AddConfiguredHttpClient<ISettingService, SettingService>();
        }

        private static void AddConfiguredHttpClient<TClient, TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
        {
            services.AddHttpClient<TClient, TImplementation>()
                    .ConfigureHttpClient(ConfigureHttpClient)
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Default is 2 mins
                    .ConfigurePrimaryHttpMessageHandler(GetHttpClientHandler);
        }

        private static void ConfigureHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            var clientConfig = serviceProvider.GetRequiredService<ITypedClientConfig>();
            httpClient.BaseAddress = clientConfig.BaseUrl;
            httpClient.Timeout = TimeSpan.FromSeconds(clientConfig.Timeout);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "BlahAgent");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                AllowAutoRedirect = false,
                UseDefaultCredentials = true,
            };
        }
    }
}
