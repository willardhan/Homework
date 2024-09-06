using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Homework.ApiHost.Interceptors;
using Homework.Infrastructure.Attributes;
using Homework.Infrastructure.Exceptions;
using Homework.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Homework.ApiHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger(Configuration);
            services.AddControllers(options => { options.Filters.Add<ExceptionFilter>(); }).AddControllersAsServices();
            services.AddFluentValidationExceptionHandler();
            services.AddCustomServices();
        }
        
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseSwaggerComponent(Configuration);
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    
    static class StartupExtension
    {
        /// <summary>
        /// add swagger
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetSection("EnabledSwagger").Value == "True")
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Demo", Version = "v1" });
                });
            }
        }
        
        /// <summary>
        /// use swagger
        /// </summary>
        public static void UseSwaggerComponent(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (configuration.GetSection("EnabledSwagger").Value == "True")
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
                });
            }
        }
        
        /// <summary>
        /// .net core DI
        /// </summary>
        public static void AddCustomServices(this IServiceCollection services)
        {
            var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var asslembyNames in refAssembyNames)
            {
                Assembly.Load(asslembyNames);
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            services.RegisterLifetimesByAttribute(assemblies, ServiceLifetime.Singleton);
            services.RegisterLifetimesByAttribute(assemblies, ServiceLifetime.Scoped);
            services.RegisterLifetimesByAttribute(assemblies, ServiceLifetime.Transient);
        }

        /// <summary>
        /// .net core DI For life time
        /// </summary>
        private static void RegisterLifetimesByAttribute(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime serviceLifetime)
        {
            List<Type> types = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(ServiceAttribute), false).Length > 0
                            && t.GetCustomAttribute<ServiceAttribute>().Lifetime == serviceLifetime
                            && t.IsClass && !t.IsAbstract)
                .ToList();
            foreach (var type in types)
            {
                Type[] interfaces = type.GetInterfaces();
                interfaces.ToList().ForEach(r =>
                {
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Singleton: services.AddSingleton(r, type); break;
                        case ServiceLifetime.Scoped: services.AddScoped(r, type); break;
                        case ServiceLifetime.Transient: services.AddTransient(r, type); break;
                    }
                });
            }
        }

        /// <summary>
        /// input valid exception filter
        /// </summary>
        public static void AddFluentValidationExceptionHandler(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errorMessage = actionContext.ModelState
                        .Where(e => e.Value != null && e.Value.Errors.HasElement())
                        .Select(e => e.Value.Errors.First().ErrorMessage)
                        .FirstOrDefault();
                    throw new InputInvalidException(errorMessage);
                };
            });
        }
    }
}