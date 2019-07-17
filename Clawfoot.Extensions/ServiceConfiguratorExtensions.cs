using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clawfoot.Extensions
{
    public static class ServiceConfiguratorExtensions
    {
        /// <summary>
        /// Ensures the provided service has already been registered.
        /// If it has not, then add it with the provided lifetime
        /// </summary>
        /// <typeparam name="TService">The Service type</typeparam>
        /// <typeparam name="TImplementation">The Service implementation</typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        public static void EnsureServiceRegistered<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            }
        }

        /// <summary>
        /// Ensures the provided <see cref="ServiceDescriptor"/> has already been registered.
        /// If it has not, then add it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="descriptor"></param>
        public static void EnsureServiceRegistered(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            if (!services.Contains(descriptor))
            {
                services.Add(descriptor);
            }
        }

        /// <summary>
        /// Determines if the generic has been registered already, regardless of the type parameters.
        /// ie. If Animal<Dog> has been registered, and you pass in Animal<Cat> this returns true
        /// </summary>
        /// <typeparam name="TGenericType">The Generic</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static bool GenericServiceDefinitionAlreadyRegistered<TGenericType>(this IServiceCollection services) where TGenericType : class
        {
            if (!typeof(TGenericType).IsGenericType)
            {
                throw new InvalidOperationException("Cannot verify a generic types registration if that type is not a generic");
            }

            Type genericDefinition = typeof(TGenericType).GetGenericTypeDefinition();


            return services.Any(x => x.ServiceType.GetGenericTypeDefinition() == genericDefinition);
        }
    }
}
