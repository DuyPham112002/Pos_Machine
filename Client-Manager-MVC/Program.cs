using Client_Manager_MVC.DependencyFactory;
using Client_Manager_MVC.HttpClientFactory;

namespace Client_Manager_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddControllersWithViews();
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(420);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //Add dependency injection
            builder.Services.AddDependencyInFactory(builder.Configuration, builder.Host);
            //Add dependency httpclient 
            builder.Services.AddHttpClientInFactory(builder.Configuration);

            var app = builder.Build();
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
