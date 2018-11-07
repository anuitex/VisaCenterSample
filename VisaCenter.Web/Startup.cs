using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisaCenter.AppHandlers;
using VisaCenter.Bus;
using VisaCenter.Interfaces.Handlers;
using VisaCenter.RabbitMqHandlers;
using VisaCenter.Repository;
using VisaCenter.Repository.Repositories;

namespace VisaCenter.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Data Source=KARFAGEN;Database=VisaCenter;Trusted_Connection=True;MultipleActiveResultSets=true"));
            services.AddTransient<ApplicationDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //SetupHanldersDependencies(services, typeof(VisaCheckHandler));
            SetupHanldersDependencies(services, typeof(RabbitMqVisaCheckHandler));

            services.AddTransient<IVisaRepository, VisaRepository>();
            services.AddTransient<IBus, AppBus>();
        }

        private void SetupHanldersDependencies(IServiceCollection services, Type sampleTypeFromAssembly) {
            var handlersAssembly = Assembly.GetAssembly(sampleTypeFromAssembly);

            foreach (var type in handlersAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<,>))
                    {
                        // NOTE: Due to a limitation of Microsoft.DependencyInjection we cannot 
                        // register an open generic interface type without also having an open generic 
                        // implementation type. So, we convert to a closed generic interface 
                        // type to register.
                        var interfaceType = typeof(IEventHandler<,>).MakeGenericType(i.GetGenericArguments()[0].GetInterfaces()[0], typeof(object));
                        services.AddTransient(typeof(IEventHandler), type);
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
