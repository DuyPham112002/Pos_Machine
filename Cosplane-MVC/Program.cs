using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cosplane_MVC_Service.AdminService;
using Cosplane_MVC_Service.AuthService;
using Cosplane_MVC_Service.BrandService;
using Cosplane_MVC_Service.DeviceService;
using Cosplane_MVC_Service.FingerprintService;

namespace Cosplane_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors();
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(480));
            // Call UseServiceProviderFactory on the Host sub property 
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            // Call ConfigureContainer on the Host sub property 
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
                builder.RegisterType<AuthService>().As<IAuthService>();
                builder.RegisterType<AdminService>().As<IAdminService>();
                builder.RegisterType<BrandService>().As<IBrandService>();
                builder.RegisterType<DeviceService>().As<IDeviceService>();
                builder.RegisterType<FingerprintService>().As<IFingerprintService>();
            });
            var app = builder.Build();
            app.UseMiddleware<BrowserInfoMiddleware>();
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}");

            app.Run();
        }
    }
}
