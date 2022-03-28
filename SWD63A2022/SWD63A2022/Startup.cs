using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SWD63A2022
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                @"C:\Users\attar\Downloads\swd63a2022-b00c3b35f560.json");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services
                     .AddAuthentication(options =>
                     {
                         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                         options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                     })
                     .AddCookie()
                     .AddGoogle(options =>
                     {
                         options.ClientId = "27946963238-c8vcqm1ba5le30dlg2v80u8icml1bqnc.apps.googleusercontent.com";
                         options.ClientSecret = "";
                     });

            services.AddRazorPages();

            string projectId = Configuration["Project"];

            services.AddScoped<FireStoreDataAccess>(
                x => {
                    return new FireStoreDataAccess(projectId);
                }
                );

            //services.AddScoped<CacheDataAccess>(
            //   x => {
            //       return new CacheDataAccess("redis-12103.c270.us-east-1-3.ec2.cloud.redislabs.com:12103,password=gdrbI9GYMVyDdq1zodHQJUQMJot9qnq6");
            //   }
            //   );

            services.AddScoped<PubsubAccess>(
            x => {
                return new PubsubAccess(projectId);
            }
            );

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
