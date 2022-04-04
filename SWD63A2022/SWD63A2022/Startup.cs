using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Google.Cloud.Diagnostics.AspNetCore3;
using Google.Cloud.Diagnostics.Common;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SWD63A2022
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment host)
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
             host.ContentRootPath +  @"/swd63a2022-b00c3b35f560.json");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string projectId = Configuration["Project"];


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

            //1. install Google.Cloud.Diagnostics.AspNetCore3 & Google.Cloud.Diagnostics.Common;

            //2.
            services.AddGoogleErrorReportingForAspNetCore(
                new Google.Cloud.Diagnostics.Common.ErrorReportingServiceOptions
            {
                // Replace ProjectId with your Google Cloud Project ID.
                ProjectId = projectId,
                // Replace Service with a name or identifier for the service.
                ServiceName = "ClassDemo",
                // Replace Version with a version for the service.
                Version = "1"
            });

            services.AddLogging(builder => builder.AddGoogle(new LoggingServiceOptions
            {
                // Replace ProjectId with your Google Cloud Project ID.
                ProjectId = projectId,
                // Replace Service with a name or identifier for the service.
                ServiceName = "ClassDemo",
                // Replace Version with a version for the service.
                Version = "1"
            }));


            services.AddControllersWithViews();

            SecretManagerServiceClient client = SecretManagerServiceClient.Create();
        
            SecretVersionName secretVersionName = new SecretVersionName(projectId, "GoogleSecretKey", "1");

            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();



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
                         options.ClientSecret = payload;
                     });

            services.AddRazorPages();

           

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
        {
            app.UseDeveloperExceptionPage();
            app.UseHsts();
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
               
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
               
            }
        }
    }
}
