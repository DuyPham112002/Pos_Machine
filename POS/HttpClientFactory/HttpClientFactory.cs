using Client_POS_Services.AuthServices;
using Client_POS_Services.ClientServices;
using Client_Services.AttributeService;
using Client_Services.CategoryServices;
using Client_Services.EmployeeServices;
using Client_Services.MenuService;
using Client_Services.IncurredService;
using Client_Services.OrderDetailServices;
using Client_Services.OrderServices;
using Client_Services.PaymentService;
using Client_Services.ProductServices;
using Client_Services.ShiftServices;
using Client_Services.SubCategoryServices;
using System.Net;
using Client_Services.SettingService;

namespace Client_POS.HttpClientFactory
{
    public static class HttpClientFactory
    {
        public static void AddHttpClientInFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguredHttpClient<IAuthService, AuthService>();
            services.AddConfiguredHttpClient<IEmployeeService, EmployeeService>();
            services.AddConfiguredHttpClient<ICategoryService, CategoryService>();
            services.AddConfiguredHttpClient<ISubCategoryService, SubCategoryService>();
            services.AddConfiguredHttpClient<IProductService, ProductService>();
            services.AddConfiguredHttpClient<IOrderService, OrderService>();
            services.AddConfiguredHttpClient<IOrderDetailService, OrderDetailService>();
            services.AddConfiguredHttpClient<IPaymentService, PaymentService>();
            services.AddConfiguredHttpClient<IAttributeService, AttributeService>();
            services.AddConfiguredHttpClient<IMenuService, MenuService>();
            services.AddConfiguredHttpClient<IShiftService, ShiftService>();
            services.AddConfiguredHttpClient<IIncurredService, IncurredService>();
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
