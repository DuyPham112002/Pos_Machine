using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cosplane_API.SignalRHub;
using Cosplane_API_DBAccess.Dapper;
using Cosplane_API_DBAccess.Entities;
using Cosplane_API_DBAccess.UnitOfWork;
using Cosplane_API_Service.AuthService;
using Cosplane_API_Service.BrandService;
using Cosplane_API_Service.DeviceService;
using Cosplane_API_Service.EmployeeService;
using Cosplane_API_Service.FingerprintService;
using Cosplane_API_Service.HashService;
using Cosplane_API_Service.RoleService;
using Cosplane_API_Service.TokenService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using System.Text;

namespace Cosplane_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors();
            builder.Services.AddControllers();
            builder.Services.AddOpenApiDocument(options =>
            {
                options.PostProcess = document =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Cosplane POS API",
                        Description = "An ASP.NET Core Web API for managing client for Cosplane POS",


                    };
                };
            }
            );

            var key = Encoding.ASCII.GetBytes("need some key here");

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

            // Call ConfigureContainer on the Host sub property 
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
                builder.RegisterType<DapperContext>().As<IDapperContext>();
                builder.RegisterType<PosmanagementContext>().AsSelf();
                builder.RegisterType<RoleService>().As<IRoleService>();
                builder.RegisterType<AccountService>().As<IAccountService>();
                builder.RegisterType<HashService>().As<IHashService>();
                builder.RegisterType<EmployeeService>().As<IEmployeeService>();
                builder.RegisterType<TokenService>().As<ITokenService>();
                builder.RegisterType<BrandService>().As<IBrandService>();
                builder.RegisterType<DeviceService>().As<IDeviceService>();
                builder.RegisterType<CheckFingerprintService>().As<ICheckFingerprintService>();
            });

            builder.Services.AddSignalR();

            var app = builder.Build();
            //if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseCors();
            }
            // Configure the HTTP request pipeline.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationHub>("/notificationHub");

            app.MapControllers();

            app.Run();
        }
    }
}
