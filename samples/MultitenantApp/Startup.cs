using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultitenantApp.Models;
using MultitenantApp.Multitenancy;
using StructureMap;
using System;
using Yeast.Multitenancy;

namespace MultitenantApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<SampleTenant, SampleTenantResolver>();

            services.AddOptions();
            services.Configure<SampleMultitenancyOptions>(Configuration.GetSection("Multitenancy"));
            
            services.ConfigureTenantServices<SampleTenant>(
                (tenant, tenantServices) =>
                {
                    // Configure EF
                    tenantServices.AddEntityFrameworkSqlServer();
                    tenantServices.AddDbContext<SampleDataContext>(options =>
                        options.UseSqlServer(tenant.ConnectionString)
                    );
                }
            );

            // Add framework services.
            services.AddMvc();

            // Use StructureMap
            var container = new Container();
            container.Populate(services);

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMultitenancy<SampleTenant>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.ConfigureTenant<SampleTenant>((tenantApp, tenant) => {
                tenantApp.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
            });

            // Fallback
            app.Run(async context => {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Tenant not found");
            });
        }
    }
}
