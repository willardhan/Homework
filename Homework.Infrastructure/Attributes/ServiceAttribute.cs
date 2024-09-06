using System;
using Microsoft.Extensions.DependencyInjection;

namespace Homework.Infrastructure.Attributes
{
    /// <summary>
    /// life time attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
        public string Named { get; set; }

        public ServiceAttribute(ServiceLifetime serviceLifetime)
        {
            Lifetime = serviceLifetime;
        }
        
        public ServiceAttribute(ServiceLifetime serviceLifetime,string named)
        {
            Lifetime = serviceLifetime;
            Named = named;
        }
    }
}

