using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ploeh.Samples.Commerce.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc();
            services.AddHttpContextAccessor();

            // Add custom Pure DI Controller Activator
            var appSettings = this.Configuration.GetSection("AppSettings");

            var controllerActivator = new CommerceControllerActivator(
                configuration: new CommerceConfiguration(
                    connectionString: this.Configuration.GetConnectionString("CommerceConnectionString"),
                    productRepositoryTypeName: appSettings.GetValue<string>("ProductRepositoryType")));

            services.AddSingleton<IControllerActivator>(controllerActivator);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            ConfigureMiddleware(app, logger);

            app.UseStaticFiles();
            app.UseCookiePolicy();

            // app.MapControllerRoute(
            //         name: "default",
            //         pattern: "{controller=Home}/{action=Index}/{id?}")
            //     .WithStaticAssets();
        }

        private static void ConfigureMiddleware(IApplicationBuilder app, ILogger logger)
        {
            // ---- Start code Section 7.3.2 ----
            app.Use(async (context, next) =>
            {
                var middleware = new LoggingMiddleware(logger);

                await middleware.Invoke(context, next);
            });
            // ---- End code Section 7.3.2 ----
        }
    }
}