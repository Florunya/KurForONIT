using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kur.Managers;
using Kur.Storeges;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kur
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //строка подключения к БД
            services.AddDbContext<FoodDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            //services.AddRouting(config =>
            //{
               // config.LowercaseQueryStrings = true;
               // config.LowercaseUrls = true;
            //});

            services.AddTransient<IFoodManager, FoodManager>();
            services.AddTransient<ICryptographyManager, CryptographyManager>();
            services.AddTransient<IWorkPortManagers, WorkPortManagers>();
            services.AddTransient<IConvertToExcel, ConvertToExcel>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Update",
                    pattern: "{controller=Home}/{action=UpdateFood}/{id?}");

                endpoints.MapControllerRoute(
                    name: "Delete",
                    pattern: "{controller=Home}/{action=DeleteFood}/{id?}");

                endpoints.MapControllerRoute(
                    name: "EncryptFile",
                    pattern: "{controller=Home}/{action=EncryptFile}");

                endpoints.MapControllerRoute(
                    name: "DecryptFile",
                    pattern: "{controller=Home}/{action=DecryptFile}");

                endpoints.MapControllerRoute(
                    name: "GetInfoPorts",
                    pattern: "{controller=Home}/{action=GetInfoPorts}");

                endpoints.MapControllerRoute(
                    name: "GetInfoActiveTCPListeners",
                    pattern: "{controller=Home}/{action=GetInfoActiveTCPListeners}");

                endpoints.MapControllerRoute(
                    name: "GetInfoActiveUDPListeners",
                    pattern: "{controller=Home}/{action=GetInfoActiveUDPListeners}");

                endpoints.MapControllerRoute(
                    name: "GetFile",
                    pattern: "{controller=Home}/{action=GetFile}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
            });
        }
    }
}
