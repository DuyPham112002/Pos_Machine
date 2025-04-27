using Autofac;
using Autofac.Extensions.DependencyInjection;
using Client_API.SignalRHub;
using Client_API_Services.AccountService;
using Client_API_Services.AttendDetailService;
using Client_API_Services.AttendService;
using Client_API_Services.AttributeService;
using Client_API_Services.AttributeSetService;
using Client_API_Services.CategoryServices;
using Client_API_Services.EmployeeService;
using Client_API_Services.Fingerprint;
using Client_API_Services.HashService;
using Client_API_Services.ImageService;
using Client_API_Services.ImageSetService;
using Client_API_Services.IncurredService;
using Client_API_Services.ManagerService;
using Client_API_Services.MenuService;
using Client_API_Services.OrderActivityLogService;
using Client_API_Services.OrderDetailService;
using Client_API_Services.OrderService;
using Client_API_Services.PaymentService;
using Client_API_Services.ProductService;
using Client_API_Services.RoleService;
using Client_API_Services.SettingService;
using Client_API_Services.ShiftService;
using Client_API_Services.StaticalService;
using Client_API_Services.SubCategoryService;
using Client_API_Services.TokenService;
using Client_DBAccess.Dapper;
using Client_DBAccess.Entities;
using Client_DBAccess.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Text;

namespace Client_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.

            string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApiDocument(options =>
            {
                options.PostProcess = document =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Cosplane POS Client API",
                        Description = "An ASP.NET Core Web API for client of Cosplane POS",


                    };
                };
                options.AddSecurity("bearer", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Bearer token authorization header",
                });

                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            }
           );

            var key = Encoding.ASCII.GetBytes("key here");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Call UseServiceProviderFactory on the Host sub property 
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // Call ConfigureContainer on the Host sub property 
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
                builder.RegisterType<DapperContext>().As<IDapperContext>();
                builder.RegisterType<PosclientContext>().AsSelf();
                builder.RegisterType<RoleService>().As<IRoleService>();
                builder.RegisterType<AccountService>().As<IAccountService>();
                builder.RegisterType<HashService>().As<IHashService>();
                builder.RegisterType<ManagerService>().As<IManagerService>();
                builder.RegisterType<EmployeeService>().As<IEmployeeService>();
                builder.RegisterType<TokenService>().As<ITokenService>();
                builder.RegisterType<ImageService>().As<IImageService>();
                builder.RegisterType<ImageSetService>().As<IImageSetService>();
                builder.RegisterType<CategoryService>().As<ICategoryService>();
                builder.RegisterType<SubCategorySerivce>().As<ISubCategorySerivce>();
                builder.RegisterType<ProductService>().As<IProductService>();
                builder.RegisterType<OrderService>().As<IOrderService>();
                builder.RegisterType<OrderDetailService>().As<IOrderDetailService>();
                builder.RegisterType<PaymentService>().As<IPaymentService>();
                builder.RegisterType<AttributeService>().As<IAttributeService>();
                builder.RegisterType<AttributeSetService>().As<IAttributeSetService>();
                builder.RegisterType<OrderActivityLogService>().As<IOrderActivityLogService>();
                builder.RegisterType<MenuService>().As<IMenuService>();
                builder.RegisterType<IncurredService>().As<IIcurredService>();
                builder.RegisterType<ShiftService>().As<IShiftService>();
                builder.RegisterType<AttendanceService>().As<IAttendanceService>();
                builder.RegisterType<AttendanceDetailService>().As<IAttendanceDetailService>();
                builder.RegisterType<GenerateFingerprint>().As<IGenerateFingerprint>();
                builder.RegisterType<StaticalService>().As<IStaticalService>();
                builder.RegisterType<SettingService>().As<ISettingService>();
            });

            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseOpenApi();

            app.UseSwaggerUi();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/notificationHub");

            app.MapControllers();

            app.Run();
        }
    }
}
