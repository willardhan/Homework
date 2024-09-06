using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Homework.Infrastructure.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Homework.UnitTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomServices();
        }
    }

    static class StartupExtension
    {
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
    }
}